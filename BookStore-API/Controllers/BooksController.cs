using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data.Models;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Endpoint used to interact with the Books in the Book Store's Database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILoggerService _loggerService;
        private readonly IMapper _mapper;
        public BooksController
            (
                IBookRepository bookRepository,
                ILoggerService loggerService,
                IMapper mapper
            )
        {
            _bookRepository = bookRepository;
            _loggerService = loggerService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get All Books
        /// </summary>
        /// <returns>List of Books</returns>
        [HttpGet]
        [Authorize(Roles = "Customer, Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks()
        {
            string location = GetControllerActionNames();
            try
            {
                _loggerService.LogInfo($"{location}: Attempted to get all books");
                var books = await _bookRepository.FindAll();
                var response = _mapper.Map<IList<BookDTO>>(books);
                _loggerService.LogInfo($"{location}: Successfully to got all books");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Get Book by ID
        /// </summary>
        /// <returns>An book´s record</returns>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Customer, Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBook(int id)
        {
            string location = GetControllerActionNames();
            try
            {
                _loggerService.LogInfo($"{location}: Attempted to get book with id:{id}");
                var book = await _bookRepository.FindById(id);
                if (book == null)
                {
                    _loggerService.LogWarn($"{location}: Book with id:{id} was not found");
                    return NotFound();
                }
                var response = _mapper.Map<BookDTO>(book);
                _loggerService.LogInfo($"{location}: Successfully got book with id:{id}");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Creates a book
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] BookCreateDTO bookDTO)
        {
            string location = GetControllerActionNames();
            try
            {
                _loggerService.LogInfo($"{location}: Book submission attempted");
                if (bookDTO == null)
                {
                    _loggerService.LogWarn($"{location}: Empty request was submitted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _loggerService.LogWarn($"{location}: Book data was incomplete");
                    return BadRequest(ModelState);
                }
                var book = _mapper.Map<Book>(bookDTO);
                var created = await _bookRepository.Create(book);
                if (!created)
                {
                    return InternalError($"{location}: Book creation failed");
                }
                _loggerService.LogInfo($"{location}: Book created successfully");
                return Created("Create", new { book });
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Update a book
        /// </summary>
        /// <param name="id"></param>
        /// <param name="book"></param>        
        /// <returns></returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] BookUpdateDTO bookDTO)
        {
            string location = GetControllerActionNames();
            try
            {
                _loggerService.LogInfo($"{location}: Book with id {id} update attempted");
                if (id < 1 || bookDTO == null || id != bookDTO.Id)
                {
                    _loggerService.LogWarn($"{location}: Book update failed with bad data");
                    return BadRequest();
                }
                var exist = await _bookRepository.Exists(id);
                if (!exist)
                {
                    _loggerService.LogWarn($"{location}: Book with id:{id} was not found");
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    _loggerService.LogWarn($"{location}: Book data was incomplete");
                    return BadRequest();
                }
                var book = _mapper.Map<Book>(bookDTO);
                var updated = await _bookRepository.Update(book);
                if (!updated)
                {
                    return InternalError($"{location}: Book update failed");
                }
                _loggerService.LogInfo($"{location}: Book with id:{id} updated successfully");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Delete Book by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            string location = GetControllerActionNames();
            try
            {
                _loggerService.LogInfo($"{location}: Book with id {id} delete attempted");
                if (id < 1)
                {
                    _loggerService.LogWarn($"{location}: Invalid book id:{id} submitted");
                    return BadRequest();
                }
                var exist = await _bookRepository.Exists(id);
                if (!exist)
                {
                    _loggerService.LogWarn($"{location}: Book with id:{id} was not found");
                    return NotFound();
                }
                var book = await _bookRepository.FindById(id);
                var deleted = await _bookRepository.Delete(book);
                if (!deleted)
                {
                    return InternalError($"{location}: Book with id:{id} delete failed");
                }
                _loggerService.LogInfo($"{location}: Book with id:{id} successfully deleted");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;
            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string message)
        {
            _loggerService.LogError(message);
            return StatusCode(500, $"Something went wrong.\nPlease contact the administrator");
        }
    }
}

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
    /// Endpoint used to interact with the Authors in the Book Store's Database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]  
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _loggerService;
        private readonly IMapper _mapper;
        public AuthorsController
            (
                IAuthorRepository authorRepository,
                ILoggerService loggerService,
                IMapper mapper
            )
        {
            _authorRepository = authorRepository;
            _loggerService = loggerService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get All Authors
        /// </summary>
        /// <returns>List of Authors</returns>
        [HttpGet]
        [Authorize(Roles = "Customer, Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]        
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            string location = GetControllerActionNames();
            try
            {
                _loggerService.LogInfo($"{location}: Attempted to get all authors");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _loggerService.LogInfo($"{location}: Successfully got all authors");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Get Author by ID
        /// </summary>
        /// <returns>An author´s record</returns>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Customer, Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            string location = GetControllerActionNames();
            try
            {
                _loggerService.LogInfo($"{location}: Attempted to get author with id:{id}");
                var author = await _authorRepository.FindById(id);
                if (author == null)
                {
                    _loggerService.LogWarn($"{location}: Author with id:{id} was not found");
                    return NotFound();
                }
                var response = _mapper.Map<AuthorDTO>(author);
                _loggerService.LogInfo($"{location}: Successfully got author with id:{id}");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Creates an author
        /// </summary>
        /// <param name="author"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDTO)
        {
            string location = GetControllerActionNames();
            try
            {
                _loggerService.LogInfo($"{location}: Author submission attempted");
                if (authorDTO == null)
                {
                    _loggerService.LogWarn($"{location}: Empty request was submitted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _loggerService.LogWarn($"{location}: Author data was incomplete");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorDTO);
                var created = await _authorRepository.Create(author);
                if (!created)
                {                   
                    return InternalError($"{location}: Author creation failed");
                }
                _loggerService.LogInfo($"{location}: Author created successfully");
                return Created("Create", new { author });
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Update an author
        /// </summary>
        /// <param name="id"></param>
        /// <param name="author"></param>        
        /// <returns></returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorDTO)
        {
            string location = GetControllerActionNames();
            try
            {
                _loggerService.LogInfo($"{location}: Author with id {id} update attempted");
                if (id < 1 || authorDTO == null || id != authorDTO.Id)
                {
                    _loggerService.LogWarn($"{location}: Author update failed with bad data");
                    return BadRequest();
                }
                var exist = await _authorRepository.Exists(id);
                if (!exist)
                {
                    _loggerService.LogWarn($"{location}: Author with id:{id} was not found");
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    _loggerService.LogWarn($"{location}: Author data was incomplete");
                    return BadRequest();
                }
                var author = _mapper.Map<Author>(authorDTO);
                var updated = await _authorRepository.Update(author);
                if (!updated)
                {
                    return InternalError($"{location}: Author update failed");
                }
                _loggerService.LogInfo($"{location}: Author with id:{id} updated successfully");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Delete Author by ID
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
                _loggerService.LogInfo($"{location}: Author with id {id} delete attempted");
                if (id < 1)
                {
                    _loggerService.LogWarn($"{location}: Invalid author id:{id} submitted");
                    return BadRequest();
                }
                var exist = await _authorRepository.Exists(id);
                if (!exist)
                {
                    _loggerService.LogWarn($"{location}: Author with id:{id} was not found");
                    return NotFound();
                }
                var author = await _authorRepository.FindById(id);                
                var deleted = await _authorRepository.Delete(author);
                if (!deleted)
                {
                    return InternalError($"{location}: Author with id:{id} delete failed");
                }
                _loggerService.LogInfo($"{location}: Author with id:{id} successfully deleted");
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

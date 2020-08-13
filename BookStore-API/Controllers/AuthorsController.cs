using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data.Models;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Endpoint used to interact with the Authors in the Book Store's Database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]    
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
        /// <returns>List of authors</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]        
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            string endpoint = "GET: api/authors";
            try
            {
                _loggerService.LogInfo($"Attempted to get all authors - {endpoint}");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _loggerService.LogInfo($"Successfully got all authors - {endpoint}");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException} - {endpoint}");
                //_loggerService.LogError($"{e.Message} - {e.InnerException} - {endpoint}");
                //return StatusCode(500, $"Something went wrong.\nPlease contact the Administrator.");
            }
        }

        /// <summary>
        /// Get Author by ID
        /// </summary>
        /// <returns>An author´s record</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            string endpoint = $"GET: api/authors/{id}";
            try
            {
                _loggerService.LogInfo($"Attempted to get author with id:{id} - {endpoint}");
                var author = await _authorRepository.FindById(id);
                if (author == null)
                {
                    _loggerService.LogWarn($"Author with id:{id} was not found - {endpoint}");
                    return NotFound();
                }
                var response = _mapper.Map<AuthorDTO>(author);
                _loggerService.LogInfo($"Successfully got author with id:{id} - {endpoint}");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException} - {endpoint}");
                //_loggerService.LogError($"{e.Message} - {e.InnerException} - {endpoint}");
                //return StatusCode(500, $"Something went wrong.\nPlease contact the Administrator");                
            }
        }

        /// <summary>
        /// Creates an author
        /// </summary>
        /// <param name="author"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDTO)
        {
            string endpoint = $"POST: api/authors";
            try
            {
                _loggerService.LogInfo($"Author submission attempted - {endpoint}");
                if (authorDTO == null)
                {
                    _loggerService.LogWarn($"Empty request was submitted - {endpoint}");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _loggerService.LogWarn($"Author data was incomplete - {endpoint}");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorDTO);
                var created = await _authorRepository.Create(author);
                if (!created)
                {                   
                    return InternalError($"Author creation failed - {endpoint}");
                }
                _loggerService.LogInfo($"Author created successfully - {endpoint}");
                return Created("Create", new { author });
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException} - {endpoint}");
            }
        }

        /// <summary>
        /// Update an author
        /// </summary>
        /// <param name="id"></param>
        /// <param name="author"></param>        
        /// <returns></returns>
        [HttpPut("{id}")]        
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorDTO)
        {
            string endpoint = $"PUT: apit/authors/{id}";
            try
            {
                _loggerService.LogInfo($"Author with id {id} update attempted - {endpoint}");
                if (id < 1 || authorDTO == null || id != authorDTO.Id)
                {
                    _loggerService.LogWarn($"Author update failed with bad data - {endpoint}");
                    return BadRequest();
                }
                var exist = await _authorRepository.Exists(id);
                if (!exist)
                {
                    _loggerService.LogWarn($"Author with id:{id} was not found - {endpoint}");
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    _loggerService.LogWarn($"Author data was incomplete - {endpoint}");
                    return BadRequest();
                }
                var author = _mapper.Map<Author>(authorDTO);
                var updated = await _authorRepository.Update(author);
                if (!updated)
                {
                    return InternalError($"Author update failed - {endpoint}");
                }
                _loggerService.LogInfo($"Author with id:{id} updated successfully - {endpoint}");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException} - {endpoint}");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            string endpoint = $"DELETE: apit/authors/{id}";
            try
            {
                _loggerService.LogInfo($"Author with id {id} delete attempted - {endpoint}");
                if (id < 1)
                {
                    _loggerService.LogWarn($"Invalid author id:{id} submitted - {endpoint}");
                    return BadRequest();
                }
                var exist = await _authorRepository.Exists(id);
                if (!exist)
                {
                    _loggerService.LogWarn($"Author with id:{id} was not found - {endpoint}");
                    return NotFound();
                }
                var author = await _authorRepository.FindById(id);                
                var deleted = await _authorRepository.Delete(author);
                if (!deleted)
                {
                    return InternalError($"Author with id:{id} delete failed");
                }
                _loggerService.LogInfo($"Author with id:{id} successfully deleted");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException} - {endpoint}");
            }
        }

        private ObjectResult InternalError(string message)
        {
            _loggerService.LogError(message);
            return StatusCode(500, $"Something went wrong.\nPlease contact the administrator");
        }

    }
}

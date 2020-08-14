using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore_API.Contracts;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookStore_API.Controllers
{
    /// <summary>
    /// This is an auto-generated API controller for testing
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILoggerService _loggerService;
        public HomeController(ILoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        /// <summary>
        /// TEST: Get 2 strings
        /// </summary>
        /// <returns></returns>
        // GET: api/<HomeController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _loggerService.LogInfo("Access to Home Controller - TESTING");
            return new string[] { "value1", "value2" };
        }

        // GET api/<HomeController>/5
        /// <summary>
        /// TEST: return "value"
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public string Get(int id)
        {
            _loggerService.LogDebug("Debugging message - TESTING");
            return "value";
        }

        // POST api/<HomeController>
        /// <summary>
        /// TEST: Does nothing
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        public void Post([FromBody] string value)
        {
            _loggerService.LogError("Error message - TESTING");
        }

        // PUT api/<HomeController>/5
        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<HomeController>/5
        /// <summary>
        /// DoesNothing
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _loggerService.LogWarn("Delete, Home Controller - TESTING");
        }
    }
}

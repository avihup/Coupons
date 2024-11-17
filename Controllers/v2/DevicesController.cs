using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using TestCase.Exceptions;
using TestCase.Interfaces.Services;
using TestCase.Models;
using TestCase.Models.ViewModels;
using TestCase.Services;

namespace TestCase.Controllers.v2
{
    [Authorize]
    [ApiController]
    [Route("api/v2/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class DevicesController : APIBaseController
    {
        [HttpGet]
        /// <summary>
        /// Retrieve Device Data
        /// </summary>
        /// <returns>The Device Data</returns>
        public IActionResult IsAlive()
        {
            return Ok(Device);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class IsAliveController : APIBaseController
    {
        [HttpGet]
        public IActionResult IsAlive()
        {
            return Ok(Device);
        }
    }
}

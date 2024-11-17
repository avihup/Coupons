using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using TestCase.Interfaces.Auth;
using TestCase.Models.Auth;
using TestCase.Models.ViewModels;

namespace TestCase.Controllers.v2
{
    [ApiController]
    [Route("api/v2/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class BindingController : ControllerBase
    {
        private readonly IBindingService _bindingService;

        public BindingController(IBindingService authService)
        {
            _bindingService = authService;
        }

        /// <summary>
        /// Device Authentication
        /// </summary>
        /// <param name="request">Device credentials</param>
        /// <returns>JWT authentication response</returns>
        [HttpPost]
        [ProducesResponseType(typeof(BindingResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<BindingResponse>> Login([FromBody] BindingRequest request)
        {
            try
            {
                var response = await _bindingService.LoginAsync(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
        }
    }
}

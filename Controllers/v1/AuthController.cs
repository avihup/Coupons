using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using TestCase.Interfaces.Auth;
using TestCase.Models.Auth;
using TestCase.Models.ViewModels;

namespace TestCase.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// User Authentication
        /// </summary>
        /// <param name="request">User credentials</param>
        /// <returns>JWT authentication response</returns>
        [HttpPost]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
        }
    }
}

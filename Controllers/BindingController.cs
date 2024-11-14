using Microsoft.AspNetCore.Mvc;
using TestCase.Interfaces.Auth;
using TestCase.Models.Auth;

namespace TestCase.Controllers
{
    [ApiController]
    [Route("api/v2/[controller]")]
    public class BindingController : ControllerBase
    {
        private readonly IBindingService _bindingService;

        public BindingController(IBindingService authService)
        {
            _bindingService = authService;
        }

        [HttpPost]
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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using TestCase.Exceptions;
using TestCase.Interfaces.Services;
using TestCase.Models.Database;
using TestCase.Models.ViewModels;

namespace TestCase.Controllers.v1
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class UsersController : AppBaseController
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a specific user by ID
        /// </summary>
        /// <param name="id">The ID of the user to retrieve</param>
        /// <returns>The requested user</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id, AppUser.ClientId);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves all users
        /// </summary>
        /// <returns>List of coupons</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync(AppUser.ClientId);
            return Ok(users);
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            try
            {
                if (AppUser.ClientId.HasValue && request.ClientId.HasValue && AppUser.ClientId != request.ClientId)
                {
                    _logger.LogWarning("Create user failed: Invalid Client Id");
                    return BadRequest(new ErrorResponse("Missing Client Id"));
                }
                request.ClientId ??= AppUser.ClientId;
                if (!request.ClientId.HasValue)
                {
                    _logger.LogWarning("Create user failed: Missing Client Id");
                    return BadRequest(new ErrorResponse("Missing Client Id"));
                }
                var user = await _userService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating coupon");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse("An unexpected error occurred while creating the user."));
            }
        }
    }

}
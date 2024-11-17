using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using TestCase.Exceptions;
using TestCase.Interfaces.Services;
using TestCase.Models;
using TestCase.Models.ViewModels;
using TestCase.Services;

namespace TestCase.Controllers.v1
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class CouponsController : AppBaseController
    {
        private readonly ICouponService _couponService;
        private readonly ILogger<CouponsController> _logger;

        public CouponsController(
            ICouponService couponService,
            ILogger<CouponsController> logger)
        {
            _couponService = couponService ?? throw new ArgumentNullException(nameof(couponService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a new coupon
        /// </summary>
        /// <param name="request">The coupon creation request</param>
        /// <returns>The created coupon</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CouponViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CouponViewModel>> Create([FromBody] CreateCouponRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new coupon. Request: {@Request}", request);

                request.ClientId ??= AppUser.ClientId;
                if (!request.ClientId.HasValue)
                {
                    _logger.LogWarning("Create coupon failed: Missing Client Id");
                    return BadRequest(new ErrorResponse("Missing Client Id"));
                }

                var result = await _couponService.CreateAsync(request);

                _logger.LogInformation("Coupon created successfully. CouponId: {CouponId}", result.Id);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Create coupon validation failed");
                return BadRequest(new ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating coupon");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse("An unexpected error occurred while creating the coupon."));
            }
        }

        /// <summary>
        /// Retrieves a specific coupon by ID
        /// </summary>
        /// <param name="id">The ID of the coupon to retrieve</param>
        /// <returns>The requested coupon</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CouponViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CouponViewModel>> GetById(Guid id)
        {
            try
            {
                _logger.LogInformation("Retrieving coupon. Id: {CouponId}, ClientId: {ClientId}",
                    id, AppUser.ClientId);

                var coupon = await _couponService.GetByIdAsync(AppUser.ClientId, id);
                if (coupon == null)
                {
                    _logger.LogWarning("Coupon not found. Id: {CouponId}", id);
                    return NotFound(new ErrorResponse($"Coupon with ID {id} not found"));
                }

                return coupon;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving coupon. Id: {CouponId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse("An unexpected error occurred while retrieving the coupon."));
            }
        }

        /// <summary>
        /// Retrieves all coupons
        /// </summary>
        /// <returns>List of coupons</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CouponViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CouponViewModel>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Retrieving all coupons for client. ClientId: {ClientId}",
                    AppUser.ClientId);

                var coupons = await _couponService.GetAllAsync(AppUser.ClientId);
                return Ok(coupons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving coupons");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse("An unexpected error occurred while retrieving coupons."));
            }
        }

        /// <summary>
        /// Cancels a specific coupon
        /// </summary>
        /// <param name="id">The ID of the coupon to cancel</param>
        /// <returns>No content on success</returns>
        [HttpPut("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Cancel(Guid id)
        {
            try
            {
                _logger.LogInformation("Cancelling coupon. Id: {CouponId}, ClientId: {ClientId}",
                    id, AppUser.ClientId);

                await _couponService.CancelAsync(AppUser.ClientId, id);

                _logger.LogInformation("Coupon cancelled successfully. Id: {CouponId}", id);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Coupon not found for cancellation. Id: {CouponId}", id);
                return NotFound(new ErrorResponse(ex.Message));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Coupon cancellation validation failed. Id: {CouponId}", id);
                return BadRequest(new ErrorResponse(ex.Message));
            }
            catch (ConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Coupon cancellation concurrency issue. Id: {CouponId}", id);
                return Conflict(new ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while cancelling coupon. Id: {CouponId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse("An unexpected error occurred while cancelling the coupon."));
            }
        }
    }
}

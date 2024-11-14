using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestCase.Exceptions;
using TestCase.Interfaces.Services;
using TestCase.Models;
using TestCase.Models.ViewModels;
using TestCase.Services;

namespace TestCase.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CouponsController : AppBaseController
    {
        private readonly ICouponService _couponService;
        private readonly ILogger<CouponsController> _logger;

        public CouponsController(ICouponService couponService,
            ILogger<CouponsController> logger)
        {
            _couponService = couponService;
            _logger = logger;
        }



        [HttpPost]
        public async Task<ActionResult<CouponViewModel>> Create([FromBody] CreateCouponRequest request)
        {
            request.ClientId ??= AppUser.ClientId;
            
            if (!request.ClientId.HasValue)
            {
                return BadRequest(new { message = "Missing Client Id" });
            }
            
            var result = await _couponService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CouponViewModel>> GetById(Guid id)
        {
            var coupon = await _couponService.GetByIdAsync(AppUser.ClientId, id);
            if (coupon == null)
                return NotFound();

            return coupon;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CouponViewModel>>> GetAll()
        {
            return Ok(await _couponService.GetAllAsync(AppUser.ClientId));
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            try
            {
                await _couponService.CancelAsync(AppUser.ClientId, id);
                return Ok();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning($"Coupon {id} Cancellation Failed", ex);
                return BadRequest(new { message = ex.Message });
            }
            catch (ConcurrencyException ex)
            {
                _logger.LogWarning($"Coupon {id} Cancellation Failed", ex);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Coupon {id} Cancellation Failed", ex);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

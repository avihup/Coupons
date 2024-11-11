using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestCase.Exceptions;
using TestCase.Models;
using TestCase.Services;
using TestCase.ViewModels;

namespace TestCase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponsController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpPost]
        public async Task<ActionResult<CouponResponse>> Create([FromBody] CreateCouponRequest request)
        {
            var result = await _couponService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CouponResponse>> GetById(string id)
        {
            var coupon = await _couponService.GetByIdAsync(id);
            if (coupon == null)
                return NotFound();

            return coupon;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CouponResponse>>> GetAll()
        {
            return Ok(await _couponService.GetAllAsync());
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(string id)
        {
            try
            {
                await _couponService.CancelAsync(id);
                return NoContent();
            }
            catch (CouponValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

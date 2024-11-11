using AutoMapper;
using MongoDB.Driver;
using TestCase.Exceptions;
using TestCase.Models;
using TestCase.Repositories;
using TestCase.Services.BackgroundServices;
using TestCase.ViewModels;

namespace TestCase.Services
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly ICouponUnitGenerationService _couponUnitGenerationService;
        private readonly ICouponUnitCancelService _couponUnitCancelService;
        private readonly IMapper _mapper;
        private readonly ILogger<CouponService> _logger;
        public CouponService(
            ICouponRepository couponRepository,
            ICouponUnitGenerationService couponUnitGenerationService,
            ICouponUnitCancelService couponUnitCancelService,
            IMapper mapper,
            ILogger<CouponService> logger)
        {
            _couponRepository = couponRepository;
            _couponUnitGenerationService = couponUnitGenerationService;
            _couponUnitCancelService = couponUnitCancelService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CouponResponse> CreateAsync(CreateCouponRequest request)
        {
            var couponDb = _mapper.Map<CouponDb>(request);
            await _couponRepository.CreateAsync(couponDb);

            await _couponUnitGenerationService.QueueCouponUnitGeneration(couponDb.Id, couponDb.TotalUnits);

            return _mapper.Map<CouponResponse>(couponDb);
        }

        public async Task<CouponResponse> GetByIdAsync(string id)
        {
            var couponDb = await _couponRepository.GetByIdAsync(id);
            return _mapper.Map<CouponResponse>(couponDb);
        }

        public async Task<IEnumerable<CouponResponse>> GetAllAsync()
        {
            var couponsDb = await _couponRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CouponResponse>>(couponsDb);
        }

        public async Task CancelAsync(string id)
        {
            try
            {
                await _couponRepository.UpdateStatusAsync(id, CouponStatus.Active, CouponStatus.Cancelled);
                await _couponUnitCancelService.QueueCouponUnitCancellation(id);
                _logger.LogInformation("Coupon {CouponId} cancelled successfully", id);
            }
            catch (CouponValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error while cancelling coupon {CouponId}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling coupon {CouponId}", id);
                throw;
            }
        }
    }
}

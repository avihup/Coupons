using AutoMapper;
using MongoDB.Driver;
using System.Data;
using TestCase.Exceptions;
using TestCase.Interfaces.Repositories;
using TestCase.Interfaces.Services;
using TestCase.Models.Database;
using TestCase.Models.ViewModels;
using TestCase.Services.BackgroundServices;

namespace TestCase.Services
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly ICouponUnitGenerationService _couponUnitGenerationService;
        private readonly ICouponUnitCancelService _couponUnitCancelService;
        private readonly IMapper _mapper;
        public CouponService(
            ICouponRepository couponRepository,
            ICouponUnitGenerationService couponUnitGenerationService,
            ICouponUnitCancelService couponUnitCancelService,
            IMapper mapper)
        {
            _couponRepository = couponRepository;
            _couponUnitGenerationService = couponUnitGenerationService;
            _couponUnitCancelService = couponUnitCancelService;
            _mapper = mapper;
        }

        public async Task<CouponViewModel> CreateAsync(CreateCouponRequest request)
        {
            var couponDb = _mapper.Map<CouponDto>(request);
            await _couponRepository.CreateAsync(couponDb);

            await _couponUnitGenerationService.QueueCouponUnitGeneration(couponDb.Id, couponDb.TotalUnits);

            return _mapper.Map<CouponViewModel>(couponDb);
        }

        public async Task<CouponViewModel> GetByIdAsync(Guid? clientId, Guid id)
        {
            var couponDb = await _couponRepository.GetByIdAsync(clientId, id);
            return _mapper.Map<CouponViewModel>(couponDb);
        }

        public async Task<IEnumerable<CouponViewModel>> GetAllAsync(Guid? clientId)
        {
            var couponsDb = await _couponRepository.GetAllAsync(clientId);
            return _mapper.Map<IEnumerable<CouponViewModel>>(couponsDb);
        }

        public async Task CancelAsync(Guid? clientId, Guid id)
        {
            // Get and validate the coupon
            var coupon = await _couponRepository.GetByIdAsync(clientId, id)
                ?? throw new ValidationException($"Coupon with ID {id} not found");

            if (coupon.Status != CouponStatus.Active)
            {
                throw new ValidationException(
                    $"Cannot cancel coupon. Current status is {coupon.Status}. Only active coupons can be cancelled."
                );
            }

            // Build update definition
            var update = Builders<CouponDto>.Update
                .Set(c => c.Status, CouponStatus.Cancelled)
                .Set(c => c.Updated, DateTime.UtcNow);

            // Perform the update
            var updated = await _couponRepository.UpdateAsync(id, update);

            if (!updated)
            {
                throw new ConcurrencyException($"Failed to cancel coupon {id}. Please try again.");
            }

            await _couponUnitCancelService.QueueCouponUnitCancellation(id);

        }
    }
}

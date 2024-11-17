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
            // Validate request
            await ValidateCreateRequest(request);

            try
            {
                var couponDb = _mapper.Map<CouponDto>(request);
                couponDb.Status = CouponStatus.Active;

                await _couponRepository.CreateAsync(couponDb);
                await _couponUnitGenerationService.QueueCouponUnitGeneration(couponDb.Id, couponDb.TotalUnits);

                return _mapper.Map<CouponViewModel>(couponDb);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                throw new ValidationException($"A coupon with name '{request.Name}' already exists for this client.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create coupon. Please try again.", ex);
            }
        }

        private async Task ValidateCreateRequest(CreateCouponRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Name))
                errors.Add("Coupon name is required.");
            else if (request.Name.Length < 3 || request.Name.Length > 100)
                errors.Add("Coupon name must be between 3 and 100 characters.");

            if (request.Amount <= 0)
                errors.Add("Amount must be greater than zero.");

            if (request.TotalUnits <= 0)
                errors.Add("Total units must be greater than zero.");

            if (request.ExpiryDate <= DateTime.UtcNow)
                errors.Add("Expiry date must be in the future.");

            if (request.ClientId == Guid.Empty)
                errors.Add("Client ID is required.");

            if (errors.Any())
                throw new ValidationException(string.Join(" ", errors));
        }

        public async Task<CouponViewModel> GetByIdAsync(Guid? clientId, Guid id)
        {
            var couponDb = await _couponRepository.GetByIdAsync(clientId, id);
            if (couponDb == null)
                throw new NotFoundException($"Coupon with ID {id} not found.");

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
                ?? throw new NotFoundException($"Coupon with ID {id} not found");

            await ValidateCancellation(coupon);

            try
            {
                // Build update definition
                var update = Builders<CouponDto>.Update
                    .Set(c => c.Status, CouponStatus.Cancelled)
                    .Set(c => c.Updated, DateTime.UtcNow);

                // Perform the update
                var updated = await _couponRepository.UpdateAsync(id, update);
                if (!updated)
                {
                    throw new ConcurrencyException($"Failed to cancel coupon {id}. The coupon may have been modified by another process.");
                }

                await _couponUnitCancelService.QueueCouponUnitCancellation(id);
            }
            catch (ConcurrencyException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while cancelling coupon {id}.", ex);
            }
        }

        private Task ValidateCancellation(CouponDto coupon)
        {
            if (coupon.Status != CouponStatus.Active)
            {
                throw new ValidationException(
                    $"Cannot cancel coupon. Current status is {coupon.Status}. Only active coupons can be cancelled."
                );
            }

            if (coupon.ExpiryDate < DateTime.UtcNow)
            {
                throw new ValidationException(
                    "Cannot cancel an expired coupon."
                );
            }

            return Task.CompletedTask;
        }
    }
}

using TestCase.Models;
using TestCase.Models.Filters;
using TestCase.Models.ViewModels;

namespace TestCase.Interfaces.Services
{
    public interface ICouponService
    {
        Task<CouponViewModel> CreateAsync(CreateCouponRequest request);
        Task<CouponViewModel> GetByIdAsync(Guid? clientId, Guid id);
        Task<IEnumerable<CouponViewModel>> GetAllAsync(Guid? clientId, CouponFilterModel filter = null);
        Task CancelAsync(Guid? clientId, Guid id);
    }
}

using TestCase.Models;
using TestCase.ViewModels;

namespace TestCase.Services
{
    public interface ICouponService
    {
        Task<CouponResponse> CreateAsync(CreateCouponRequest request);
        Task<CouponResponse> GetByIdAsync(string id);
        Task<IEnumerable<CouponResponse>> GetAllAsync();
        Task CancelAsync(string id);
    }
}

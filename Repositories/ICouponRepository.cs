using TestCase.Models;

namespace TestCase.Repositories
{
    public interface ICouponRepository
    {
        Task<CouponDb> CreateAsync(CouponDb coupon);
        Task<CouponDb> GetByIdAsync(string id);
        Task<IEnumerable<CouponDb>> GetAllAsync();
        Task UpdateStatusAsync(string id, CouponStatus oldStatus, CouponStatus newStatus);
    }
}

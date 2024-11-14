using MongoDB.Driver;
using TestCase.Models.Database;

namespace TestCase.Interfaces.Repositories
{
    public interface ICouponRepository
    {
        Task<CouponDto> CreateAsync(CouponDto coupon);
        Task<CouponDto> GetByIdAsync(Guid? clientId, Guid id);
        Task<IEnumerable<CouponDto>> GetAllAsync(Guid? clientId);
        Task<bool> UpdateAsync(Guid id, UpdateDefinition<CouponDto> update);
    }
}

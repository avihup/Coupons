using MongoDB.Driver;
using TestCase.Exceptions;
using TestCase.Interfaces.Repositories;
using TestCase.Models.Database;

namespace TestCase.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly IMongoCollection<CouponDto> _couponsCollection;

        public CouponRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("CouponsDb");
            _couponsCollection = database.GetCollection<CouponDto>("Coupons");
        }

        public async Task<CouponDto> CreateAsync(CouponDto coupon)
        {
            await _couponsCollection.InsertOneAsync(coupon);
            return coupon;
        }

        public async Task<CouponDto> GetByIdAsync(Guid? clientId, Guid id)
        {
            var filter = Builders<CouponDto>.Filter.And(
                Builders<CouponDto>.Filter.Eq(c => c.Id, id),
                Builders<CouponDto>.Filter.Eq(c => c.ClientId, clientId)
            );

            return await _couponsCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CouponDto>> GetAllAsync(Guid? clientId)
        {
            var filter = Builders<CouponDto>.Filter.And(
                Builders<CouponDto>.Filter.Eq(c => c.ClientId, clientId)
            );
            return await _couponsCollection.Find(filter).ToListAsync();
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateDefinition<CouponDto> update)
        {
            var result = await _couponsCollection.UpdateOneAsync(
                c => c.Id == id,
                update
            );

            return result.ModifiedCount > 0;
        }
    }
}

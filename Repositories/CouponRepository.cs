using MongoDB.Driver;
using TestCase.Exceptions;
using TestCase.Models;

namespace TestCase.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly IMongoCollection<CouponDb> _couponsCollection;

        public CouponRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("CouponsDb");
            _couponsCollection = database.GetCollection<CouponDb>("Coupons");
        }

        public async Task<CouponDb> CreateAsync(CouponDb coupon)
        {
            await _couponsCollection.InsertOneAsync(coupon);
            return coupon;
        }

        public async Task<CouponDb> GetByIdAsync(string id)
        {
            return await _couponsCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CouponDb>> GetAllAsync()
        {
            return await _couponsCollection.Find(_ => true).ToListAsync();
        }

        public async Task UpdateStatusAsync(string id, CouponStatus oldStatus, CouponStatus newStatus)
        {
            var filter = Builders<CouponDb>.Filter.And(
                Builders<CouponDb>.Filter.Eq(c => c.Id, id),
                Builders<CouponDb>.Filter.Eq(c => c.Status, oldStatus)
            );

            var update = Builders<CouponDb>.Update
                .Set(c => c.Status, newStatus)
                .Set(c => c.Updated, DateTime.UtcNow);

            var result = await _couponsCollection.UpdateOneAsync(filter, update);

            if (result.ModifiedCount == 0)
            {
                var coupon = await GetByIdAsync(id);
                if (coupon == null)
                {
                    throw new CouponValidationException($"Coupon with ID {id} not found");
                }
                throw new CouponValidationException(
                    $"Cannot cancel coupon. Current status is {coupon.Status}. Only active coupons can be cancelled."
                );
            }
        }
    }
}

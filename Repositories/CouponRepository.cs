using MongoDB.Driver;
using TestCase.Exceptions;
using TestCase.Interfaces.Repositories;
using TestCase.Models.Database;
using TestCase.Models.Filters;

namespace TestCase.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly IMongoCollection<CouponDto> _couponsCollection;

        public CouponRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("CouponsDb");
            _couponsCollection = database.GetCollection<CouponDto>("Coupons");
            ConfigureCollection().GetAwaiter().GetResult();
        }

        private async Task ConfigureCollection()
        {
            // Create compound unique index for Name + ClientId
            var indexKeysDefinition = Builders<CouponDto>.IndexKeys
                .Ascending(x => x.ClientId)
                .Ascending(x => x.Name);

            var indexOptions = new CreateIndexOptions
            {
                Unique = true,
                Name = "ClientId_Name_Unique"
            };

            await _couponsCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<CouponDto>(indexKeysDefinition, indexOptions));
        }

        public async Task<CouponDto> CreateAsync(CouponDto coupon)
        {
            try
            {
                await _couponsCollection.InsertOneAsync(coupon);
                return coupon;
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new InvalidOperationException(
                    $"A coupon with name '{coupon.Name}' already exists for client {coupon.ClientId}");
            }
        }

        public async Task<CouponDto> GetByIdAsync(Guid? clientId, Guid id)
        {
            var idFilter = Builders<CouponDto>.Filter.Eq(c => c.Id, id);
            var filter = clientId.HasValue
                ? Builders<CouponDto>.Filter.And(
                    idFilter,
                    Builders<CouponDto>.Filter.Eq(c => c.ClientId, clientId.Value))
                : idFilter;
            return await _couponsCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CouponDto>> GetAllAsync(Guid? clientId, CouponFilterModel filter = null)
        {
            var builder = Builders<CouponDto>.Filter;
            var filters = new List<FilterDefinition<CouponDto>>();
            // Add client filter if provided
            if (clientId.HasValue)
            {
                filters.Add(builder.Eq(x => x.ClientId, clientId.Value));
            }

            // Add other filters based on the filter model
            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.Name))
                {
                    filters.Add(builder.Regex(x => x.Name,
                        new MongoDB.Bson.BsonRegularExpression(filter.Name, "i")));
                }

                if (filter.MinAmount.HasValue)
                {
                    filters.Add(builder.Gte(x => x.Amount, filter.MinAmount.Value));
                }

                if (filter.MaxAmount.HasValue)
                {
                    filters.Add(builder.Lte(x => x.Amount, filter.MaxAmount.Value));
                }

                if (filter.StartDate.HasValue)
                {
                    filters.Add(builder.Gte(x => x.ExpiryDate, filter.StartDate.Value));
                }

                if (filter.EndDate.HasValue)
                {
                    filters.Add(builder.Lte(x => x.ExpiryDate, filter.EndDate.Value));
                }

                if (filter.Status.HasValue)
                {
                    filters.Add(builder.Eq(x => x.Status, filter.Status.Value));
                }

                if (filter.Type.HasValue)
                {
                    filters.Add(builder.Eq(x => x.Type, filter.Type.Value));
                }
            }
            var combinedFilter = filters.Any() ? builder.And(filters) : builder.Empty;
            return await _couponsCollection.Find(combinedFilter).ToListAsync();
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateDefinition<CouponDto> update)
        {
            try
            {
                // Add Updated timestamp to any update operation
                update = update.Set(x => x.Updated, DateTime.UtcNow);

                var result = await _couponsCollection.UpdateOneAsync(
                    c => c.Id == id,
                    update
                );

                return result.ModifiedCount > 0;
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new InvalidOperationException(
                    "Update failed: The new coupon name already exists for this client");
            }
        }

        // Helper method for common update operations
        public async Task<bool> UpdateCouponAsync(Guid id, Action<UpdateDefinitionBuilder<CouponDto>> updateAction)
        {
            var updateBuilder = Builders<CouponDto>.Update;
            var updateDefinition = updateBuilder.Combine();
            updateAction(updateBuilder);

            return await UpdateAsync(id, updateDefinition);
        }
    }
}

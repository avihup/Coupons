using MongoDB.Bson;
using MongoDB.Driver;
using TestCase.Interfaces.Repositories;
using TestCase.Models.Database;

namespace TestCase.Repositories
{
    public class KiosksRespository : IKiosksRespository
    {
        private readonly IMongoCollection<KioskDto> _collection;

        public KiosksRespository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("CouponsDb");
            _collection = database.GetCollection<KioskDto>("Kiosks");
        }
        public async Task<KioskDto> GetByAccessToken(string accessToken)
        {
            var pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("AccessToken", accessToken)),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "Clients" },
                    { "let", new BsonDocument("clientId", "$ClientId") },
                    { "pipeline", new BsonArray
                        {
                            new BsonDocument("$match", new BsonDocument(
                                "$expr", new BsonDocument(
                                    "$eq", new BsonArray { "$_id", "$$clientId" }
                                )
                            ))
                        }
                    },
                    { "as", "Client" }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$Client" },
                    { "preserveNullAndEmptyArrays", true }
                })
            };

            return await _collection
                .Aggregate<KioskDto>(pipeline, new AggregateOptions { AllowDiskUse = true })
                .FirstOrDefaultAsync();
        }

        public async Task<KioskDto> GetByIdAsync(Guid id)
        {
            return await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();
        }
    }
}

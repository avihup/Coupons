using MongoDB.Bson;
using MongoDB.Driver;
using TestCase.Interfaces.Repositories;
using TestCase.Models.Database;

namespace TestCase.Repositories
{
    public class MachinesRepository : IMachinesRepository
    {
        private readonly IMongoCollection<MachineDto> _collection;

        public MachinesRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("CouponsDb");
            _collection = database.GetCollection<MachineDto>("Machines");
        }
        public async Task<MachineDto> GetByAccessToken(string accessToken)
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
                .Aggregate<MachineDto>(pipeline, new AggregateOptions { AllowDiskUse = true })
                .FirstOrDefaultAsync();
        }

        public async Task<MachineDto> GetByIdAsync(Guid id)
        {
            return await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();
        }
    }
}

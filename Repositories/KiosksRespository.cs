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
            return await _collection.Find(c => c.AccessToken == accessToken).FirstOrDefaultAsync();
        }

        public async Task<KioskDto> GetByIdAsync(Guid id)
        {
            return await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();
        }
    }
}

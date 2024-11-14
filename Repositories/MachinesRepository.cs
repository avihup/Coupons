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
            return await _collection.Find(c => c.AccessToken == accessToken).FirstOrDefaultAsync();
        }

        public async Task<MachineDto> GetByIdAsync(Guid id)
        {
            return await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();
        }
    }
}

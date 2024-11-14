using MongoDB.Driver;
using TestCase.Interfaces.Repositories;
using TestCase.Models.Database;

namespace TestCase.Repositories
{
    public class ClientsRepository : IClientsRepository
    {
        private readonly IMongoCollection<ClientDto> _clientsCollection;

        public ClientsRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("CouponsDb");
            _clientsCollection = database.GetCollection<ClientDto>("Clients");
        }

        public async Task<ClientDto> GetByIdAsync(Guid id)
        {
            return await _clientsCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
        }
    }
}

using MongoDB.Driver;
using TestCase.Interfaces.Repositories;
using TestCase.Models.Database;

namespace TestCase.Repositories
{
    public class UsersRespository : IUsersRespository
    {
        private readonly IMongoCollection<UserDto> _usersCollection;

        public UsersRespository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("CouponsDb");
            _usersCollection = database.GetCollection<UserDto>("Users");
        }
        public async Task<UserDto> GetByIdAsync(Guid id)
        {
            return await _usersCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<UserDto> GetUserNameAsync(string userName)
        {
            return await _usersCollection.Find(c => c.UserName == userName).FirstOrDefaultAsync();
        }
    }
}

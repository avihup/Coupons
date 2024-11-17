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
            ConfigureCollection().GetAwaiter().GetResult();
        }

        private async Task ConfigureCollection()
        {
            // Create compound unique index for Name + ClientId
            var indexKeysDefinition = Builders<UserDto>.IndexKeys
                .Ascending(x => x.UserName);

            var indexOptions = new CreateIndexOptions
            {
                Unique = true,
                Name = "UserName_Unique"
            };

            await _usersCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<UserDto>(indexKeysDefinition, indexOptions));
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

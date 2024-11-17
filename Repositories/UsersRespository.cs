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
        public async Task<UserDto> GetByIdAsync(Guid id, Guid? clientId = null)
        {
            var idFilter = Builders<UserDto>.Filter.Eq(c => c.Id, id);
            var filter = clientId.HasValue
                ? Builders<UserDto>.Filter.And(
                    idFilter,
                    Builders<UserDto>.Filter.Eq(c => c.ClientId, clientId.Value))
                : idFilter;
            return await _usersCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<UserDto> GetUserNameAsync(string userName)
        {
            return await _usersCollection.Find(c => c.UserName == userName).FirstOrDefaultAsync();
        }

        public async Task<UserDto> CreateAsync(UserDto user)
        {
            try
            {
                await _usersCollection.InsertOneAsync(user);
                return user;
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new InvalidOperationException(
                    $"A user with name '{user.UserName}' already exists");
            }
           
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync(Guid? clientId = null)
        {
            var filter = clientId.HasValue
                ? Builders<UserDto>.Filter.Eq(c => c.ClientId, clientId.Value)
                : Builders<UserDto>.Filter.Empty;
            return await _usersCollection.Find(filter).ToListAsync();
        }
    }
}

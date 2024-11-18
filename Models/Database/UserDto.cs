using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TestCase.Models.Database
{
    public class UserDto : BaseDto
    {
        [BsonRepresentation(BsonType.String)]
        public Guid? ClientId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }

        [BsonIgnoreIfNull]
        public ClientDto Client { get; set; }
    }
}

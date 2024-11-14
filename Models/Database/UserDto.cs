using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TestCase.Models.Database
{
    public class UserDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid? ClientId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public DateTime Created { get; set; }
    }
}

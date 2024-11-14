using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TestCase.Models.Database
{
    public class KioskDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]

        public Guid ClientId { get; set; }

        public string AccessToken { get; set; }

        public string Name { get; set; }
        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }
    }
}

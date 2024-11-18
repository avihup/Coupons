using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TestCase.Models.Database
{
    public class KioskDto : BaseDto
    {
        [BsonRepresentation(BsonType.String)]

        public Guid ClientId { get; set; }

        public string AccessToken { get; set; }

        public string Name { get; set; }

        [BsonIgnoreIfNull]
        public ClientDto Client { get; set; }
    }
}

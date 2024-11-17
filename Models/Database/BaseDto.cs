using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TestCase.Models.Database
{
    public class BaseDto
    {
        public BaseDto()
        {
            Id = Guid.NewGuid();
            Created = DateTime.UtcNow;
            Updated = DateTime.UtcNow;
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonElement("Created")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Created { get; private set; }
        [BsonElement("Updated")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? Updated { get; private set; }


    }
}

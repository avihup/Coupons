using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TestCase.Models.Database
{
    public class MachineDto : BaseDto
    {
        [BsonRepresentation(BsonType.String)]

        public Guid ClientId { get; set; }
        public string Name { get; set; }
        public string AccessToken { get; set; }
    }
}

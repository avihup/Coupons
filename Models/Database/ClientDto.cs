using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TestCase.Models.Database
{
    public class ClientDto: BaseDto
    {
        public string Name { get; set; }
    }
}

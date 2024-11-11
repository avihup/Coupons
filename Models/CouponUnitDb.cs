using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TestCase.Models
{
    public class CouponUnitDb
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string CouponId { get; set; }

        public Guid? OrderId { get; set; }
        public CouponStatus Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}

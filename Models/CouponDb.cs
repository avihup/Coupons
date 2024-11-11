using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TestCase.Models
{
    public class CouponDb
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime ExpiryDate { get; set; }
        public CouponType Type { get; set; }
        public decimal Amount { get; set; }
        public CouponStatus Status { get; set; }
        public int TotalUnits { get; set; }
        
        public DateTime? Updated { get; set; }
    }
}

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TestCase.Models.Database
{
    public class CouponUnitDto : BaseDto
    {
        [BsonRepresentation(BsonType.String)]
        public Guid CouponId { get; set; }

        public Guid? OrderId { get; set; }
        public CouponStatus Status { get; set; }
    }
}

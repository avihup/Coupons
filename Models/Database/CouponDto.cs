using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace TestCase.Models.Database
{
    public class CouponDto: BaseDto
    {
        [BsonRepresentation(BsonType.String)]
        public Guid ClientId { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Name { get; set; }

        [BsonElement("ExpiryDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime ExpiryDate { get; set; }

        public CouponType Type { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        public CouponStatus Status { get; set; }

        [Range(0, int.MaxValue)]
        public int TotalUnits { get; set; }
    }
}

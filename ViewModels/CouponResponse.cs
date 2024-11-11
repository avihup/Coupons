using TestCase.Models;

namespace TestCase.ViewModels
{
    public class CouponResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime ExpiryDate { get; set; }
        public CouponType Type { get; set; }
        public decimal Amount { get; set; }
        public CouponStatus Status { get; set; }
        public int TotalUnits { get; set; }
        public Guid? OrderId { get; set; }
        public DateTime? Updated { get; set; }
    }

}

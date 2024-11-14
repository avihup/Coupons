using TestCase.Models.Database;

namespace TestCase.Models.ViewModels
{
    public class CouponViewModel
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
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

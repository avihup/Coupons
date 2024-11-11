using System.ComponentModel.DataAnnotations;
using TestCase.Models;
using TestCase.Validation;

namespace TestCase.ViewModels
{
    public class CreateCouponRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [FutureDate]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public CouponType Type { get; set; }

        [Required]
        [Range(0.01, 10000)]
        public decimal Amount { get; set; }

        [Required]
        [Range(1, 100000)]
        public int TotalUnits { get; set; }

    }
}

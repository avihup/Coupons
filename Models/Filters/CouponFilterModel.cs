using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TestCase.Models.Database;

namespace TestCase.Models.Filters
{
    public class CouponFilterModel
    {
        /// <summary>
        /// Filter coupons by name (partial match, case-insensitive)
        /// </summary>
        /// <example>Summer</example>
        [FromQuery(Name = "name")]
        public string? Name { get; set; }

        /// <summary>
        /// Filter coupons with amount greater than or equal to this value
        /// </summary>
        /// <example>10.00</example>
        [FromQuery(Name = "minAmount")]
        public decimal? MinAmount { get; set; }

        /// <summary>
        /// Filter coupons with amount less than or equal to this value
        /// </summary>
        /// <example>100.00</example>
        [FromQuery(Name = "maxAmount")]
        [Range(0, double.MaxValue, ErrorMessage = "MaxAmount must be greater than or equal to 0")]
        public decimal? MaxAmount { get; set; }

        /// <summary>
        /// Filter coupons expiring after this date
        /// </summary>
        /// <example>2024-01-01</example>
        [FromQuery(Name = "startDate")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Filter coupons expiring before this date
        /// </summary>
        /// <example>2024-12-31</example>
        [FromQuery(Name = "endDate")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Filter coupons by status
        /// </summary>
        /// <example>Active</example>
        [FromQuery(Name = "status")]
        public CouponStatus? Status { get; set; }

        /// <summary>
        /// Filter coupons by type
        /// </summary>
        /// <example>Active</example>
        [FromQuery(Name = "type")]
        public CouponType? Type { get; set; }

        /// <summary>
        /// Validates the filter model
        /// </summary>
        public void Validate()
        {
            var errors = new List<string>();

            if (MinAmount.HasValue && MaxAmount.HasValue && MinAmount > MaxAmount)
            {
                errors.Add("MinAmount cannot be greater than MaxAmount");
            }

            if (StartDate.HasValue && EndDate.HasValue && StartDate > EndDate)
            {
                errors.Add("StartDate cannot be later than EndDate");
            }

            if (errors.Any())
            {
                throw new ValidationException(string.Join(" ", errors));
            }
        }
    }
}

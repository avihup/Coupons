using MongoDB.Driver;
using TestCase.Models.Database;

namespace TestCase.Extensions
{
    public static class CouponUpdateExtensions
    {
        public static UpdateDefinition<CouponDto> Combine(
            this UpdateDefinitionBuilder<CouponDto> builder,
            params UpdateDefinition<CouponDto>[] updates)
        {
            return builder.Combine(updates);
        }
    }
}

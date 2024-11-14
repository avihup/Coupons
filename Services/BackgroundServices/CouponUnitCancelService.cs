using MongoDB.Driver;
using System.Threading.Channels;
using TestCase.Models.Database;

namespace TestCase.Services.BackgroundServices
{
    public interface ICouponUnitCancelService
    {
        Task QueueCouponUnitCancellation(Guid couponId);
    }

    public class CouponUnitCancelService : BackgroundService, ICouponUnitCancelService
    {
        private readonly IMongoCollection<CouponUnitDto> _couponUnitsCollection;
        private readonly ILogger<CouponUnitCancelService> _logger;
        private readonly Channel<Guid> _channel;

        public CouponUnitCancelService(
            IMongoClient mongoClient,
            ILogger<CouponUnitCancelService> logger)
        {
            var database = mongoClient.GetDatabase("CouponsDb");
            _couponUnitsCollection = database.GetCollection<CouponUnitDto>("CouponUnits");
            _logger = logger;
            _channel = Channel.CreateUnbounded<Guid>();
        }

        public async Task QueueCouponUnitCancellation(Guid couponId)
        {
            await _channel.Writer.WriteAsync(couponId);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var couponId = await _channel.Reader.ReadAsync(stoppingToken);
                try
                {

                    // Define the filter for active coupon units with the specified couponId
                    var filter = Builders<CouponUnitDto>.Filter.And(
                        Builders<CouponUnitDto>.Filter.Eq(u => u.CouponId, couponId),
                        Builders<CouponUnitDto>.Filter.Eq(u => u.Status, CouponStatus.Active)
                    );

                    // Define the update to set status to cancelled and update timestamp
                    var update = Builders<CouponUnitDto>.Update
                        .Set(u => u.Status, CouponStatus.Cancelled)
                        .Set(u => u.Updated, DateTime.UtcNow);

                    // Execute the update
                    var result = await _couponUnitsCollection.UpdateManyAsync(
                        filter,
                        update,
                        new UpdateOptions { IsUpsert = false },
                        stoppingToken
                    );

                    _logger.LogInformation(
                        "Cancelled {ModifiedCount} active units for coupon {CouponId}",
                        result.ModifiedCount,
                        couponId
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing coupon unit cancellation for coupon {CouponId}", couponId);
                }
            }
        }
    }
}

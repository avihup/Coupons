using MongoDB.Driver;
using System.Threading.Channels;
using TestCase.Models;

namespace TestCase.Services.BackgroundServices
{
    public interface ICouponUnitGenerationService
    {
        Task QueueCouponUnitGeneration(string couponId, int units);
    }

    public class CouponUnitGenerationService : BackgroundService, ICouponUnitGenerationService
    {
        private readonly IMongoCollection<CouponUnitDb> _couponUnitsCollection;
        private readonly ILogger<CouponUnitGenerationService> _logger;
        private readonly Channel<(string CouponId, int Units)> _channel;

        public CouponUnitGenerationService(
            IMongoClient mongoClient,
            ILogger<CouponUnitGenerationService> logger)
        {
            var database = mongoClient.GetDatabase("CouponsDb");
            _couponUnitsCollection = database.GetCollection<CouponUnitDb>("CouponUnits");
            _logger = logger;
            _channel = Channel.CreateUnbounded<(string, int)>();
        }

        public async Task QueueCouponUnitGeneration(string couponId, int units)
        {
            await _channel.Writer.WriteAsync((couponId, units));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var (couponId, units) = await _channel.Reader.ReadAsync(stoppingToken);
                    var couponUnits = new List<CouponUnitDb>();

                    for (int i = 0; i < units; i++)
                    {
                        couponUnits.Add(new CouponUnitDb
                        {
                            CouponId = couponId,
                            Status = CouponStatus.Active,
                            Created = DateTime.UtcNow
                        });
                    }

                    await _couponUnitsCollection.InsertManyAsync(couponUnits, cancellationToken: stoppingToken);
                    _logger.LogInformation($"Generated {units} units for coupon {couponId}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing coupon unit generation");
                }
            }
        }
    }
}

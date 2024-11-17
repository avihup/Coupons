using StackExchange.Redis;
using TestCase.Interfaces.Services;
using TestCase.Services;

namespace TestCase.Extensions
{
    public static class RedisServiceExtensions
    {
        public static IServiceCollection AddRedisCache(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var multiplexer = ConnectionMultiplexer.Connect(
                configuration.GetConnectionString("Redis") ?? "localhost:6379");

            services.AddSingleton<IConnectionMultiplexer>(multiplexer);
            services.AddSingleton<IRedisService, RedisService>();

            return services;
        }
    }
}

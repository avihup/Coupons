using TestCase.Interfaces.Repositories;
using TestCase.Interfaces.Services;
using TestCase.Models.Database;
using Microsoft.Extensions.DependencyInjection;

namespace TestCase.Middlewares
{
    public class ApiAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRedisService _redisService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ApiAuthenticationMiddleware(
            RequestDelegate next,
            IRedisService redisService,
            IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _redisService = redisService;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            if (path?.StartsWith("/api/v2/") == true && !path.StartsWith("/api/v2/binding"))
            {
                var deviceType = context.User.Claims.FirstOrDefault(c => c.Type == "deviceType")?.Value;

                if (string.IsNullOrEmpty(deviceType) || (deviceType != "Kiosk" && deviceType != "Machine"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                var deviceId = context.User.Claims.FirstOrDefault(c => c.Type == "deviceId")?.Value;
                if (string.IsNullOrEmpty(deviceId))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    switch (deviceType)
                    {
                        case "Kiosk":
                            var kiosk = await _redisService.GetAsync<KioskDto>($"{deviceType}-{deviceId}");
                            if (kiosk == null)
                            {
                                var kiosksRepository = scope.ServiceProvider.GetRequiredService<IKiosksRespository>();
                                kiosk = await kiosksRepository.GetByIdAsync(Guid.Parse(deviceId));
                                if (kiosk == null)
                                {
                                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                    return;
                                }
                                await _redisService.SetAsync($"{deviceType}-{deviceId}", kiosk, TimeSpan.FromHours(1));
                            }
                            break;

                        case "Machine":
                            var machine = await _redisService.GetAsync<MachineDto>($"{deviceType}-{deviceId}");
                            if (machine == null)
                            {
                                var machinesRepository = scope.ServiceProvider.GetRequiredService<IMachinesRepository>();
                                machine = await machinesRepository.GetByIdAsync(Guid.Parse(deviceId));
                                if (machine == null)
                                {
                                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                    return;
                                }
                                await _redisService.SetAsync($"{deviceType}-{deviceId}", machine, TimeSpan.FromHours(1));
                            }
                            break;

                        default:
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return;
                    }
                }
            }

            await _next(context);
        }
    }

    public static class ApiAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiAuthenticationMiddleware>();
        }
    }
}
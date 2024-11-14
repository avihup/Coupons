namespace TestCase.Middlewares
{
    // simple middleware checking that the authenticated token is kiosk or machine
    public class ApiAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
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
            }

            await _next(context);
        }
    }

    // Extension method for easy middleware registration
    public static class ApiAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiAuthenticationMiddleware>();
        }
    }
}

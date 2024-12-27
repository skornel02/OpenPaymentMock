using System.Security.Claims;
using Microsoft.Extensions.Options;
using OpenPaymentMock.Server.Options;
using OpenPaymentMock.Server.Security;

namespace OpenPaymentMock.Server.Middlewares;

public static class ApiKeyMiddleware
{
    public static void UseApiKeyMiddleware(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            var options = context.RequestServices.GetRequiredService<IOptions<AdminOptions>>().Value;

            if (context.Request.Headers.TryGetValue(ApiKeySecurityConstants.ApiKeyHeader, out var apiKey) && apiKey == options.ApiKey)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Role, ApiKeySecurityConstants.ApiKeyRole)
                };

                var identity = new ClaimsIdentity(claims, ApiKeySecurityConstants.ApiKeySchema);

                context.User.AddIdentity(identity);
            }

            await next(context);
        });
    }
}

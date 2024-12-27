using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using OpenPaymentMock.Server.Persistance;
using OpenPaymentMock.Server.Security;

namespace OpenPaymentMock.Server.Middlewares;

public static class PartnerAccessKeyMiddleware
{
    public static void UsePartnerAccessKeyMiddleware(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();

            if (context.Request.Headers.TryGetValue(PartnerAccessKeySecurityConstants.ApiKeyHeader, out var apiKey))
            {
                var accessKey = await dbContext.PartnerAccessKeys
                    .AsNoTracking()
                    .Where(_ => _.Key == apiKey.ToString())
                    .FirstOrDefaultAsync();

                if (accessKey is not null
                    && !accessKey.Deleted
                    && (!accessKey.ExpiresAt.HasValue || accessKey.ExpiresAt < DateTime.UtcNow))
                {
                    var validLogin = true;

                    if (context.Request.RouteValues.TryGetValue("paymentId", out var paymentIdObj)
                        && paymentIdObj is Guid paymentId)
                    {
                        var payment = await dbContext.PaymentSituations
                            .AsNoTracking()
                            .SingleOrDefaultAsync(p => p.Id == paymentId);

                        if (payment?.PartnerId != accessKey.PartnerId)
                        {
                            validLogin = false;
                        }
                    }

                    if (context.Request.Query.TryGetValue("partnerId", out var partnerIdObj) && Guid.TryParse(partnerIdObj, out var partnerId))
                    {
                        if (partnerId != accessKey.PartnerId)
                        {
                            validLogin = false;
                        }
                    }

                    if (validLogin)
                    {

                        var claims = new List<Claim>
                        {
                            new(ClaimTypes.NameIdentifier, accessKey.PartnerId.ToString()),
                            new(ClaimTypes.Role, PartnerAccessKeySecurityConstants.ApiKeyRole)
                        };

                        var identity = new ClaimsIdentity(claims, "PartnerAccessKey");

                        context.User.AddIdentity(identity);

                        await dbContext.PartnerAccessKeys
                            .Where(_ => _.Id == accessKey.Id)
                            .ExecuteUpdateAsync(_ => _.SetProperty(_ => _.LastUsed, DateTime.UtcNow)
                                .SetProperty(_ => _.UsageCount, _ => _.UsageCount + 1));
                    }
                }
            }

            await next(context);
        });
    }
}

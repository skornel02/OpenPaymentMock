using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using OpenPaymentMock.Server.Security;

namespace OpenPaymentMock.Server.Filters;

public static class EndpointRoleFilter
{
    public static RouteHandlerBuilder UseRoleFilter(this RouteHandlerBuilder endpoint, params string[] role)
    {
        endpoint.AddEndpointFilter(async (context, next)
            => role.Any(r => context.HttpContext.User.IsInRole(r))
                ? await next(context)
                : Results.Problem("Unauthorized", statusCode: 401))
            .Produces<ProblemDetails>(statusCode: 401)
            .WithOpenApi(_ =>
            {
                OpenApiSecurityScheme? apiKeySchema = role.Contains(ApiKeySecurityConstants.ApiKeyRole)
                    ? new()
                    {
                        Reference = new()
                        {
                            Id = ApiKeySecurityConstants.ApiKeySchema,
                            Type = ReferenceType.SecurityScheme,
                        }
                    }
                    : null;

                OpenApiSecurityScheme? partnerKeySchema = role.Contains(PartnerAccessKeySecurityConstants.ApiKeyRole)
                    ? new()
                    {
                        Reference = new()
                        {
                            Id = PartnerAccessKeySecurityConstants.ApiKeySchema,
                            Type = ReferenceType.SecurityScheme
                        }
                    }
                    : null;

                if (apiKeySchema is not null)
                {
                    _.Security.Add(new OpenApiSecurityRequirement()
                    {
                        [apiKeySchema] = []
                    });
                }

                if (partnerKeySchema is not null)
                {
                    _.Security.Add(new OpenApiSecurityRequirement()
                    {
                        [partnerKeySchema] = []
                    });
                }

                return _;
            })
            .WithTags(role.Select(_ => $"_Role {_}").ToArray());

        return endpoint;
    }
}

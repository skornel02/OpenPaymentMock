using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using OpenPaymentMock.Server.Security;

namespace OpenPaymentMock.Server.Extensions;

public static class SwaggerExtensions
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(_ =>
        {
            _.SupportNonNullableReferenceTypes();

            _.MapType<TimeSpan>(() => new OpenApiSchema
            {
                Type = "string",
                Example = new OpenApiString("00:00:00")
            });

            _.AddSecurityDefinition(ApiKeySecurityConstants.ApiKeySchema, new()
            {
                Type = SecuritySchemeType.ApiKey,
                Scheme = "apiKey",
                Name = ApiKeySecurityConstants.ApiKeyHeader,
                In = ParameterLocation.Header,
            });
            _.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new()
                        {
                            Reference = new()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = ApiKeySecurityConstants.ApiKeySchema
                            }
                        },
                        []
                    }
                });

            _.AddSecurityDefinition(PartnerAccessKeySecurityConstants.ApiKeySchema, new()
            {
                Type = SecuritySchemeType.ApiKey,
                Scheme = "apiKey",
                Name = PartnerAccessKeySecurityConstants.ApiKeyHeader,
                In = ParameterLocation.Header,
            });
            _.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new()
                        {
                            Reference = new()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = PartnerAccessKeySecurityConstants.ApiKeySchema
                            }
                        },
                        []
                    }
                });
        });
    }

    public static void UseSwaggerConfig(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(_ =>
        {
            _.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
            _.ConfigObject.AdditionalItems.Add("tagsSorter", "alpha");
        });
    }
}

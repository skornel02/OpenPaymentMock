using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using OpenPaymentMock.Communication.Partners;
using OpenPaymentMock.Server.Extensions;
using OpenPaymentMock.Server.Persistance;

namespace OpenPaymentMock.Server.Endpoints;

public static class PartnerEndpoints
{
    public static void MapPartnerEndpoints(this IEndpointRouteBuilder builder)
    {
        var endpoints = builder.MapGroup("/partners");

        endpoints.MapGet("", async Task<Results<Ok<List<PartnerShortDto>>, BadRequest>> (ApplicationDbContext context) =>
        {
            var partners = await context.Partners
                .ToShortDto()
                .ToListAsync();

            return TypedResults.Ok(partners);
        });

        endpoints.MapPost("", async Task<Results<Created<PartnerShortDto>, BadRequest>> (
            PartnerCreationDto dto, 
            ApplicationDbContext context) =>
        {
            var entity = dto.ToEntity();

            await context.Partners.AddAsync(entity);
            await context.SaveChangesAsync();

            return TypedResults.Created($"/api/partners/{entity.Id}", entity.ToShortDto());
        });
    }
}

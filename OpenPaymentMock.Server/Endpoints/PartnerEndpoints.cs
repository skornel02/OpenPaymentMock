using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using OpenPaymentMock.Communication.PartnerAccessKeys;
using OpenPaymentMock.Communication.Partners;
using OpenPaymentMock.Server.Extensions;
using OpenPaymentMock.Server.Filters;
using OpenPaymentMock.Server.Persistance;

namespace OpenPaymentMock.Server.Endpoints;

public static class PartnerEndpoints
{
    public static void MapPartnerEndpoints(this IEndpointRouteBuilder builder)
    {
        var endpoints = builder.MapGroup("/partners")
            .WithTags("Partners");

        endpoints.MapGet("", async Task<Results<Ok<List<PartnerShortDto>>, UnauthorizedHttpResult>> (
            ApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            var partners = await context.Partners
                .ToShortDto()
                .ToListAsync(cancellationToken);

            return TypedResults.Ok(partners);
        })
            .UseRoleFilter("Admin");

        endpoints.MapPost("", async Task<Results<Created<PartnerShortDto>, BadRequest>> (
            PartnerCreationDto dto,
            ApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            var entity = dto.ToEntity();

            await context.Partners.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return TypedResults.Created($"/api/partners/{entity.Id}", entity.ToShortDto());
        })
            .UseRoleFilter("Admin");

        endpoints.MapGet("/{id}", async Task<Results<Ok<PartnerShortDto>, NotFound>> (
            Guid id,
            ApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            var partner = await context.Partners
                .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);

            return partner is null ? (Results<Ok<PartnerShortDto>, NotFound>)TypedResults.NotFound() : (Results<Ok<PartnerShortDto>, NotFound>)TypedResults.Ok(partner.ToShortDto());
        })
            .UseRoleFilter("Admin");

        endpoints.MapDelete("/{id}", async Task<Results<NoContent, NotFound>> (
            Guid id,
            ApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            var partner = await context.Partners
                .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (partner is null)
            {
                return TypedResults.NotFound();
            }

            context.Partners.Remove(partner);
            await context.SaveChangesAsync(cancellationToken);

            return TypedResults.NoContent();
        })
            .UseRoleFilter("Admin")
            .WithSummary("Warning! This deletes the partner permanently. This can only happen if there are no payments for this partner.");

        endpoints.MapGet("/{id:guid}/access-keys", async Task<Results<Ok<List<PartnerAccessKeyDetailsDto>>, NotFound>> (
            Guid id,
            ApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            var keys = await context.PartnerAccessKeys
                .AsNoTracking()
                .Where(_ => _.PartnerId == id)
                .ToDetailedDto()
                .ToListAsync(cancellationToken);

            return TypedResults.Ok(keys);
        })
            .UseRoleFilter("Admin");

        endpoints.MapPost("/{id:guid}/access-keys", async Task<Results<Ok<PartnerAccessKeyDetailsWithSecretDto>, NotFound>> (
            Guid id,
            PartnerAccessKeyCreationDto request,
            ApplicationDbContext context,
            CancellationToken cancellationToken
            ) =>
        {
            var partner = await context.Partners
                .AsNoTracking()
                .Where(_ => _.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            if (partner is null)
            {
                return TypedResults.NotFound();
            }

            var accessKeyEntity = request.ToEntity(id);

            await context.PartnerAccessKeys.AddAsync(accessKeyEntity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            accessKeyEntity.Partner = partner;

            return TypedResults.Ok(accessKeyEntity.ToDetailedWithKeyDto());
        })
            .UseRoleFilter("Admin");

        endpoints.MapDelete("/{id:guid}/access-keys/{accessKeyId:guid}", async Task<Results<NoContent, NotFound>> (
            Guid id,
            Guid accessKeyId,
            ApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            var accessKey = await context.PartnerAccessKeys
                .Where(_ => _.Id == accessKeyId && _.PartnerId == id)
                .FirstOrDefaultAsync(cancellationToken);

            if (accessKey is null)
            {
                return TypedResults.NotFound();
            }

            accessKey.Deleted = true;

            await context.SaveChangesAsync(cancellationToken);

            return TypedResults.NoContent();
        })
            .UseRoleFilter("Admin")
            .WithSummary("Marks the access key as deleted. Can no longer be used permanently.");
    }
}

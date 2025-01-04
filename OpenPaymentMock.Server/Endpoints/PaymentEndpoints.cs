using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenPaymentMock.Communication.Payment;
using OpenPaymentMock.Communication.Payments;
using OpenPaymentMock.Server.Extensions;
using OpenPaymentMock.Server.Filters;
using OpenPaymentMock.Server.Options;
using OpenPaymentMock.Server.Persistance;

namespace OpenPaymentMock.Server.Endpoints;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this IEndpointRouteBuilder builder)
    {
        var endpoints = builder.MapGroup("/payments")
            .WithTags("Payment");

        endpoints.MapGet("", async Task<Results<Ok<List<PaymentSituationDetailsDto>>, ProblemHttpResult>> (
            Guid? partnerId,
            HttpContext httpContext,
            ApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            if (!httpContext.User.IsInRole("Admin") && partnerId is null)
            {
                return TypedResults.Problem("Unauthorized", statusCode: 401);
            }

            var payments = await context.PaymentSituations
                .AsNoTracking()
                .Where(_ => partnerId == null || _.PartnerId == partnerId)
                .ToDetailedDto()
                .ToListAsync(cancellationToken);

            return TypedResults.Ok(payments);
        })
            .UseRoleFilter("Admin", "Partner");

        endpoints.MapPost("", async Task<Results<Created<PaymentCreatedDto>, BadRequest>> (
            [FromBody] PaymentSituationCreationDto dto,
            [FromQuery] Guid partnerId,
            ApplicationDbContext context,
            IOptions<ApplicationOptions> options,
            CancellationToken cancellationToken) =>
        {
            var entity = dto.ToEntity(partnerId);

            await context.PaymentSituations.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            var detailedDto = context.PaymentSituations
                .AsNoTracking()
                .Single(_ => _.Id == entity.Id)
                .ToDetailedDto();

            var baseUrl = new Uri(options.Value.ApplicationUrl);
            var paymentUrl = new Uri(baseUrl, $"payments/{detailedDto.Id.ToString()}");
            var createdDto = new PaymentCreatedDto(detailedDto, paymentUrl.ToString());

            return TypedResults.Created($"/api/payments/{entity.Id}", createdDto);
        })
            .UseRoleFilter("Admin", "Partner");

        endpoints.MapGet("/{paymentId:guid}", async Task<Results<Ok<PaymentSituationDetailsDto>, NotFound>> (
            Guid paymentId,
            ApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            var payment = await context.PaymentSituations
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == paymentId, cancellationToken);

            return payment is null ? (Results<Ok<PaymentSituationDetailsDto>, NotFound>)TypedResults.NotFound() : (Results<Ok<PaymentSituationDetailsDto>, NotFound>)TypedResults.Ok(payment.ToDetailedDto());
        })
            .UseRoleFilter("Admin", "Partner");

        endpoints.MapGet("/{paymentId:guid}/attempts", async Task<Results<Ok<List<PaymentAttemptDetailsDto>>, NotFound>> (
            Guid paymentId,
            ApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            var attempts = await context.PaymentAttempts
                .Where(a => a.PaymentSituationId == paymentId)
                .ToDetailedDto()
                .ToListAsync(cancellationToken);

            return TypedResults.Ok(attempts);
        })
            .UseRoleFilter("Admin", "Partner");
    }
}

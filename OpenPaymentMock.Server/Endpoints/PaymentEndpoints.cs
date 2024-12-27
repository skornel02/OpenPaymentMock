using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenPaymentMock.Communication.Payment;
using OpenPaymentMock.Communication.Payments;
using OpenPaymentMock.Server.Extensions;
using OpenPaymentMock.Server.Filters;
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

        endpoints.MapPost("", async Task<Results<Created<PaymentSituationDetailsDto>, BadRequest>> (
            [FromBody] PaymentSituationCreationDto dto,
            [FromQuery] Guid partnerId,
            ApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            var entity = dto.ToEntity(partnerId);

            await context.PaymentSituations.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return TypedResults.Created($"/api/payments/{entity.Id}", entity.ToDetailedDto());
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

            if (payment is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(payment.ToDetailedDto());
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

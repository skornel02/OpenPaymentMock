using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenPaymentMock.Communication.Payment;
using OpenPaymentMock.Communication.Payments;
using OpenPaymentMock.Server.Extensions;
using OpenPaymentMock.Server.Persistance;

namespace OpenPaymentMock.Server.Endpoints;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this IEndpointRouteBuilder builder)
    {
        var endpoints = builder.MapGroup("/payments");

        endpoints.MapGet("", async Task<Results<Ok<List<PaymentSituationDetailsDto>>, BadRequest>> (ApplicationDbContext context) =>
        {
            var payments = await context.PaymentSituations
                .ToDetailedDto()
                .ToListAsync();

            return TypedResults.Ok(payments);
        });

        endpoints.MapPost("", async Task<Results<Created<PaymentSituationDetailsDto>, BadRequest>> (
            [FromBody] PaymentSituationCreationDto dto,
            [FromQuery] Guid partnerId,
            ApplicationDbContext context) =>
        {
            var entity = dto.ToEntity(partnerId);

            await context.PaymentSituations.AddAsync(entity);
            await context.SaveChangesAsync();

            return TypedResults.Created($"/api/payments/{entity.Id}", entity.ToDetailedDto());
        });

        // todo: Current attempt. Complex.

        // todo: All attempts for payment.

        // todo: current attempt input endpoints. State machine shenanigens.
    }
}

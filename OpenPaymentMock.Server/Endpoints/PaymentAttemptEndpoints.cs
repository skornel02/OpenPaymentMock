using AsyncKeyedLock;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenPaymentMock.Communication.Payments;
using OpenPaymentMock.Model.Entities;
using OpenPaymentMock.Model.Enums;
using OpenPaymentMock.Model.Extensions;
using OpenPaymentMock.Server.Extensions;
using OpenPaymentMock.Server.Persistance;

namespace OpenPaymentMock.Server.Endpoints;

public static class PaymentAttemptEndpoints
{
    private static readonly AsyncKeyedLocker<Guid> PaymentAttemptLocker = new();

    public static void MapPaymentAttemptEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/payments/{paymentId}")
            .WithTags("Payment Attempt");

        group.MapGet("/current-attempt", async Task<Results<Ok<CurrentPaymentAttemptDto>, ProblemHttpResult>> (
            Guid paymentId,
            ApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            using var _ = await PaymentAttemptLocker.LockAsync(paymentId);

            var paymentSituation = await context.PaymentSituations
                .AsNoTracking()
                .Include(_ => _.PaymentAttempts)
                .Include(_ => _.Partner)
                .Where(_ => _.Id == paymentId)
                .FirstOrDefaultAsync(cancellationToken);

            if (paymentSituation is null)
            {
                return TypedResults.Problem("Payment not found!", statusCode: 404);
            }

            var paymentAttempts = paymentSituation.PaymentAttempts
                .OrderByDescending(_ => _.CreatedAt)
                .ToList();

            var currentAttempt = paymentAttempts.FirstOrDefault();

            if (!paymentSituation.Status.IsFinalized()
                && (currentAttempt is null || currentAttempt.Status.IsFinalized()))
            {
                currentAttempt = new PaymentAttemptEntity
                {
                    Id = Guid.NewGuid(),
                    PaymentSituationId = paymentId,
                    CreatedAt = DateTime.UtcNow,
                    Status = PaymentAttemptStatus.NotAttempted
                };

                await context.PaymentAttempts.AddAsync(currentAttempt, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            }

            if (currentAttempt is null)
            {
                return TypedResults.Problem("Payment is finished without attempt!", statusCode: 500);
            }

            var currentAttemptDto = await context.PaymentAttempts
                .AsNoTracking()
                .Where(_ => _.Id == currentAttempt!.Id)
                .ToCurrentDto()
                .FirstAsync(cancellationToken);

            return TypedResults.Ok(currentAttemptDto);
        })
            .ProducesProblem(404);

        group.MapPost("/attempts/{attemptId:guid}/start", async Task<Results<Accepted, ProblemHttpResult>> (
            Guid paymentId,
            Guid attemptId,
            ApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            var attempt = await context.PaymentAttempts
                .Include(_ => _.PaymentSituation)
                .Where(_ => _.PaymentSituationId == paymentId && _.Id == attemptId)
                .FirstOrDefaultAsync(cancellationToken);

            if (attempt is null)
            {
                return TypedResults.Problem("Payment not found!", statusCode: 404);
            }

            if (attempt.Status != PaymentAttemptStatus.NotAttempted)
            {
                return TypedResults.Problem("Payment cannot be started!", statusCode: 400);
            }

            attempt.Status = PaymentAttemptStatus.Started;

            if (attempt.PaymentSituation.Status == PaymentSituationStatus.Created)
            {
                attempt.PaymentSituation.Status = PaymentSituationStatus.Processing;
            }

            await context.SaveChangesAsync(cancellationToken);

            return TypedResults.Accepted($"/payments/{paymentId}/current-attempt");
        });

        group.MapPost("/attempts/{attemptId:guid}/paid-successfully", async Task<Results<Accepted, ProblemHttpResult>> (
            Guid paymentId,
            Guid attemptId,
            ApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            var attempt = await context.PaymentAttempts
                .Include(_ => _.PaymentSituation)
                .Where(_ => _.PaymentSituationId == paymentId && _.Id == attemptId)
                .FirstOrDefaultAsync(cancellationToken);

            if (attempt is null)
            {
                return TypedResults.Problem("Payment not found!", statusCode: 404);
            }

            if (attempt.Status != PaymentAttemptStatus.Started)
            {
                return TypedResults.Problem("Payment cannot be completed!", statusCode: 400);
            }

            attempt.Status = PaymentAttemptStatus.Succeeded;
            attempt.FinishedAt = DateTime.UtcNow;

            attempt.PaymentSituation.Status = PaymentSituationStatus.Succeeded;
            attempt.PaymentSituation.FinishedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(cancellationToken);

            return TypedResults.Accepted($"/payments/{paymentId}/current-attempt");
        });

        group.MapPost("/attempts/{attemptId:guid}/payment-cancelled", async Task<Results<Accepted, ProblemHttpResult>> (
            Guid paymentId,
            Guid attemptId,
            ApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            var attempt = await context.PaymentAttempts
                .Include(_ => _.PaymentSituation)
                .Where(_ => _.PaymentSituationId == paymentId && _.Id == attemptId)
                .FirstOrDefaultAsync(cancellationToken);

            if (attempt is null)
            {
                return TypedResults.Problem("Payment not found!", statusCode: 404);
            }

            if (attempt.Status != PaymentAttemptStatus.Started)
            {
                return TypedResults.Problem("Payment cannot be cancelled!", statusCode: 400);
            }

            attempt.Status = PaymentAttemptStatus.PaymentError;
            attempt.PaymentError = "Payment was cancelled by the user";
            attempt.FinishedAt = DateTime.UtcNow;

            attempt.PaymentSituation.Status = PaymentSituationStatus.Failed;
            attempt.PaymentSituation.FinishedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(cancellationToken);

            return TypedResults.Accepted($"/payments/{paymentId}/current-attempt");
        });

        group.MapPost("/attempts/{attemptId:guid}/payment-issue", async Task<Results<Accepted, ProblemHttpResult>> (
            Guid paymentId,
            Guid attemptId,
            [FromQuery] string? error,
            ApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            var attempt = await context.PaymentAttempts
                .Include(_ => _.PaymentSituation)
                .Where(_ => _.PaymentSituationId == paymentId && _.Id == attemptId)
                .FirstOrDefaultAsync(cancellationToken);

            if (attempt is null)
            {
                return TypedResults.Problem("Payment not found!", statusCode: 404);
            }

            if (attempt.Status != PaymentAttemptStatus.Started)
            {
                return TypedResults.Problem("Payment cannot be cancelled!", statusCode: 400);
            }

            attempt.Status = PaymentAttemptStatus.PaymentError;
            attempt.PaymentError = error;
            attempt.FinishedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(cancellationToken);

            return TypedResults.Accepted($"/payments/{paymentId}/current-attempt");
        });
    }
}

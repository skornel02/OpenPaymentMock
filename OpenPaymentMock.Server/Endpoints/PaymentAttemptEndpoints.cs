using AsyncKeyedLock;
using Microsoft.AspNetCore.Http.HttpResults;
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
        var group = builder.MapGroup("/payments/{paymentId:guid}")
            .WithTags("Payment Attempt");

        group.MapGet("/current-attempt", async Task<Results<Ok<PaymentAttemptDetailsDto>, ProblemHttpResult>> (
            Guid paymentId,
            ApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            using var _ = await PaymentAttemptLocker.LockAsync(paymentId);

            var paymentSituation = await context.PaymentSituations
                .AsNoTracking()
                .Include(_ => _.PaymentAttempts)
                .Where(_ => _.Id == paymentId)
                .FirstOrDefaultAsync(cancellationToken);

            if (paymentSituation is null)
            {
                return TypedResults.Problem("Payment not found!", statusCode: 404);
            }

            if (paymentSituation.Status.IsFinalized())
            {
                return TypedResults.Problem("Payment is finalized!", statusCode: 400);
            }

            var paymentAttempts = paymentSituation.PaymentAttempts
                .OrderByDescending(_ => _.CreatedAt)
                .ToList();

            var currentAttempt = paymentAttempts.FirstOrDefault();

            if (currentAttempt is null || currentAttempt.Status.IsFinalized())
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

            return TypedResults.Ok(currentAttempt.ToDetailedDto());
        })
            .ProducesProblem(400)
            .ProducesProblem(404);

        group.MapPost("/attempts/{attemptId:guid}/start", async Task<Results<Ok<PaymentAttemptDetailsDto>, ProblemHttpResult>> (
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

            return TypedResults.Ok(attempt.ToDetailedDto());
        });

        group.MapPost("/attempts/{attemptId:guid}/paid-successfully", async Task<Results<Ok<PaymentAttemptDetailsDto>, ProblemHttpResult>> (
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

            return TypedResults.Ok(attempt.ToDetailedDto());
        });

        group.MapPost("/attempts/{attemptId:guid}/payment-cancelled", async Task<Results<Ok<PaymentAttemptDetailsDto>, ProblemHttpResult>> (
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

            return TypedResults.Ok(attempt.ToDetailedDto());
        });
    }
}

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenPaymentMock.Communication.Callback;
using OpenPaymentMock.Server.Filters;
using OpenPaymentMock.Server.Interfaces;
using OpenPaymentMock.Server.Options;

namespace OpenPaymentMock.Server.Endpoints;

public static class CallbackTestEndpoints
{
    public static void MapCallbackTestEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/test-callback", async Task<Results<Ok<PaymentCallbackTestResultDto>, ProblemHttpResult>> (
            [FromBody] PaymentCallbackTestRequestDto request,
            [FromQuery] Guid partnerId,
            ICallbackService callbackService,
            CancellationToken cancellationToken) =>
        {
            if (!Uri.TryCreate(request.CallbackUrl, UriKind.Absolute, out var url))
            {
                return TypedResults.Problem(detail: "Invalid callback url",
                    statusCode: 400);
            }

            var (success, error) = await callbackService.SendCallbackToServiceAsync(new()
            {
                CallbackUrl = request.CallbackUrl,
                Amount = 0,
                CreatedAt = DateTimeOffset.MinValue,
                Currency = "Test",
                Id = Guid.Empty,
                PartnerId = Guid.Empty,
                RedirectUrl = string.Empty,
                Status = Model.Enums.PaymentSituationStatus.Processing,
                TimeoutAt = DateTimeOffset.MinValue,
                Secret = "Callback test",
            }, cancellationToken);

            return TypedResults.Ok(new PaymentCallbackTestResultDto(success, error));
        })
            .WithTags("Callback")
            .UseRoleFilter("Admin", "Partner");
    }
}

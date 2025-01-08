using OpenPaymentMock.Model.Entities;
using OpenPaymentMock.Model.Enums;
using OpenPaymentMock.Server.StateMachines;
using Stateless.Graph;

namespace OpenPaymentMock.Server.Endpoints;

public static class StateMachineVisualizationEndpoints
{
    private static readonly Lazy<string> PaymentAttemptMermaid = new(static () =>
    {
        var entity = new PaymentAttemptEntity()
        {
            CreatedAt = DateTimeOffset.UtcNow,
            TimeoutAt = DateTimeOffset.UtcNow.AddMinutes(5),
            Id = Guid.Empty,
            PaymentSituationId = Guid.Empty,
            Status = PaymentAttemptStatus.NotAttempted
        };

        var stateMachine = entity.GetStateMachine(out _);

        return MermaidGraph.Format(stateMachine.GetInfo());
    });

    private static readonly Lazy<string> PaymentSituationMermaid = new(static () =>
    {
        var entity = new PaymentSituationEntity()
        {
            CreatedAt = DateTimeOffset.UtcNow,
            TimeoutAt = DateTimeOffset.UtcNow,
            Id = Guid.Empty,
            Status = PaymentSituationStatus.Created,
            Amount = 0,
            CallbackUrl = string.Empty,
            Currency = string.Empty,
            PartnerId = Guid.Empty,
            RedirectUrl = string.Empty,
        };
        var stateMachine = entity.GetStateMachine();
        return MermaidGraph.Format(stateMachine.GetInfo());
    });

    public static void MapStateMachineVisualizationEndpoints(this IEndpointRouteBuilder builder)
    {
        var endpoints = builder.MapGroup("/statemachines")
            .WithTags("StateMachines");

        var mermaid = endpoints.MapGroup("/mermaid");

        mermaid.MapGet("/payment-attempt", (
            CancellationToken cancellationToken) =>
        {
            return TypedResults.Text(PaymentAttemptMermaid.Value);
        });

        mermaid.MapGet("/payment-situation", (
            CancellationToken cancellationToken) =>
        {
            return TypedResults.Text(PaymentSituationMermaid.Value);
        });
    }
}

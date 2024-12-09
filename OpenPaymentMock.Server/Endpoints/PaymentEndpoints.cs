using MassTransit;

namespace OpenPaymentMock.Server.Endpoints;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this IEndpointRouteBuilder builder)
    {
        var endpoints = builder.MapGroup("/payments");

        NewId.NextGuid();
    }
}

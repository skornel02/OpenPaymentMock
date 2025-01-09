namespace OpenPaymentMock.Server.Endpoints;

public static class ApiEndpoints
{
    public static void MapApiEndpoints(this IEndpointRouteBuilder builder)
    {
        var api = builder.MapGroup("/api")
            .WithOpenApi();

        api.MapPartnerEndpoints();
        api.MapPaymentEndpoints();
        api.MapPaymentAttemptEndpoints();
        api.MapCallbackTestEndpoints();
    }
}

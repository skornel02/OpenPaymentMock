namespace OpenPaymentMock.Server.Endpoints;

public static class ApiEndpoints
{
    public static void MapApiEndpoints(this IEndpointRouteBuilder builder)
    {
        var api = builder.MapGroup("/api");

        api.MapPartnerEndpoints();
        api.MapPaymentEndpoints();
    }
}

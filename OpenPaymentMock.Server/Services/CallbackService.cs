using OpenPaymentMock.Model.Entities;
using OpenPaymentMock.Server.Extensions;
using OpenPaymentMock.Server.Interfaces;

namespace OpenPaymentMock.Server.Services;

public class CallbackService : ICallbackService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CallbackService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<(bool success, string? latestResponse)> SendCallbackToServiceAsync(PaymentSituationEntity paymentSituation, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient();

        var callbackDto = paymentSituation.ToCallbackDto();

        try
        {
            var response = await client.PostAsJsonAsync(paymentSituation.CallbackUrl, callbackDto, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            return (response.IsSuccessStatusCode, $"{response.StatusCode}:" + body);
        }
        catch (Exception ex)
        {
            return (false, $"ERROR:{ex.Message}");
        }
    }
}

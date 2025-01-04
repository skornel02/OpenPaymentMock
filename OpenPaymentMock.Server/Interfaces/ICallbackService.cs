using OpenPaymentMock.Model.Entities;

namespace OpenPaymentMock.Server.Interfaces;

public interface ICallbackService
{
    Task<(bool success, string? latestResponse)> SendCallbackToServiceAsync(PaymentSituationEntity paymentSituation, CancellationToken cancellationToken = default);
}

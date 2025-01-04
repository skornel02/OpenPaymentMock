using OpenPaymentMock.Model.Enums;

namespace OpenPaymentMock.Model.Entities;

public class PaymentCallbackEntity
{
    public required Guid Id { get; set; }

    public required string CallbackUrl { get; set; }

    public PaymentCallbackStatus Status { get; set; } = PaymentCallbackStatus.Pending;

    public required Guid PaymentSituationId { get; set; }

    public string? LatestResponse { get; set; }

    public DateTimeOffset? LatestResponseAt { get; set; }

    public int RetryCount { get; set; }

    public PaymentSituationEntity PaymentSituation { get; set; } = null!;
}

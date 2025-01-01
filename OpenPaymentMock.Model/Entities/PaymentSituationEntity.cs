using OpenPaymentMock.Model.Enums;
using OpenPaymentMock.Model.Options;

namespace OpenPaymentMock.Model.Entities;

public class PaymentSituationEntity
{
    public required Guid Id { get; set; }

    public required PaymentSituationStatus Status { get; set; } = PaymentSituationStatus.Created;

    public required decimal Amount { get; set; }

    public required string Currency { get; set; }

    public required string CallbackUrl { get; set; }

    public required string RedirectUrl { get; set; }

    public required TimeSpan Timeout { get; set; }

    public required DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTimeOffset? FinishedAt { get; set; }

    public PaymentOptions PaymentOptions { get; set; } = new();

    public required Guid PartnerId { get; set; }

    public PartnerEntity Partner { get; set; } = null!;

    public List<PaymentAttemptEntity> PaymentAttempts { get; set; } = null!;
}

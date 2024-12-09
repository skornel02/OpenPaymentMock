using OpenPaymentMock.Model.Enums;

namespace OpenPaymentMock.Model.Entities;

public class PaymentSituationEntity
{
    public required Guid Id { get; set; }

    public required PaymentSituationStatus Status { get; set; } = PaymentSituationStatus.Created; 

    public required decimal Amount { get; set; }

    public required string Currency { get; set; }

    public required string CallbackUrl { get; set; }

    public required TimeSpan Timeout { get; set; }

    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? FinishedAt { get; set; }

    public required Guid PartnerId { get; set; }

    public PartnerEntity Partner { get; set; } = null!;

    public List<PaymentAttemptEntity> PaymentAttempts { get; set; } = null!;
}

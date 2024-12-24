using OpenPaymentMock.Model.Enums;

namespace OpenPaymentMock.Communication.Payments;
public record PaymentAttemptDetailsDto(
    Guid Id,
    PaymentAttemptStatus Status,
    string? PaymentError,
    DateTime CreatedAt,
    DateTime? FinishedAt,
    Guid PaymentSituationId
);

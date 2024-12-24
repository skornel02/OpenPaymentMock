using System.ComponentModel.DataAnnotations;
using OpenPaymentMock.Model.Enums;

namespace OpenPaymentMock.Communication.Payments;

public record PaymentSituationDetailsDto(
    Guid Id,
    PaymentSituationStatus Status,
    decimal Amount,
    string Currency,
    string CallbackUrl,
    TimeSpan Timeout,
    DateTime CreatedAt,
    DateTime? FinishedAt,
    Guid PartnerId
);

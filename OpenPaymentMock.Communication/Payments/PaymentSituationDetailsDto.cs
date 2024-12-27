using System.ComponentModel.DataAnnotations;
using OpenPaymentMock.Model.Enums;
using OpenPaymentMock.Model.Options;

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
    PaymentOptions PaymentOptions,
    Guid PartnerId
);

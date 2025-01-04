using OpenPaymentMock.Model.Enums;
using OpenPaymentMock.Model.Options;

namespace OpenPaymentMock.Communication.Payments;

public record PaymentSituationDetailsDto(
    Guid Id,
    PaymentSituationStatus Status,
    decimal Amount,
    string Currency,
    string CallbackUrl,
    string RedirectUrl,
    TimeSpan Timeout,
    DateTimeOffset CreatedAt,
    DateTimeOffset? FinishedAt,
    PaymentOptions PaymentOptions,
    Guid PartnerId,
    PaymentCallbackStatus? CallbackStatus
);

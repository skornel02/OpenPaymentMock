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
    DateTimeOffset CreatedAt,
    DateTimeOffset TimeoutAt,
    DateTimeOffset? FinishedAt,
    PaymentOptions PaymentOptions,
    Guid PartnerId
);

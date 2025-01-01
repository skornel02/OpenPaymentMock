using OpenPaymentMock.Model.Enums;
using OpenPaymentMock.Model.Options;

namespace OpenPaymentMock.Communication.Payments;

public record PaymentSituationPublicDto(
    Guid Id,
    PaymentSituationStatus Status,
    decimal Amount,
    string Currency,
    string CallbackUrl,
    string RedirectUrl,
    DateTimeOffset? FinishedAt,
    PaymentOptions PaymentOptions,
    string PartnerName
);

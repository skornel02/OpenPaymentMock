using OpenPaymentMock.Model.Options;

namespace OpenPaymentMock.Communication.Payment;

public record PaymentSituationCreationDto(
    decimal Amount,
    string Currency,
    string CallbackUrl,
    string RedirectUrl,
    string? Secret,
    TimeSpan Timeout,
    PaymentOptions? PaymentOptions
);

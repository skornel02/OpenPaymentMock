namespace OpenPaymentMock.Communication.Payment;

public record PaymentSituationCreationDto(
    decimal Amount, 
    string Currency,
    string CallbackUrl,
    TimeSpan Timeout
);

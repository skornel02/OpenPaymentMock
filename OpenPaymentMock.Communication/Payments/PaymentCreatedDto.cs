namespace OpenPaymentMock.Communication.Payments;

public record PaymentCreatedDto(
    PaymentSituationDetailsDto Payment,
    string RedirectUrl
);

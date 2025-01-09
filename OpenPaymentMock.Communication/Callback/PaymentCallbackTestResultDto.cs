namespace OpenPaymentMock.Communication.Callback;

public record PaymentCallbackTestResultDto(
    bool Success,
    string? ErrorMessage
);

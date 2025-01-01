namespace OpenPaymentMock.Model.Options;

public sealed class PaymentOptions
{
    public bool AllowInvalidCards { get; set; } = false;

    public bool GenerateRandomCardDetails { get; set; } = true;
}

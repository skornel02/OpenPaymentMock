using OpenPaymentMock.Model.Enums;

namespace OpenPaymentMock.Communication.Callback;
public class PaymentCallbackDto
{
    public required Guid Id { get; set; }

    public required PaymentSituationStatus Result { get; set; }

    public string? Secret { get; set; }
}

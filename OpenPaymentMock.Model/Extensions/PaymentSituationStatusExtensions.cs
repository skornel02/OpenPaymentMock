using OpenPaymentMock.Model.Enums;

namespace OpenPaymentMock.Model.Extensions;
public static class PaymentSituationStatusExtensions
{
    public static bool IsFinalized(this PaymentSituationStatus status) => status switch
    {
        PaymentSituationStatus.Succeeded
            or PaymentSituationStatus.Failed
            or PaymentSituationStatus.Cancelled
            or PaymentSituationStatus.Refunded
            => true,
        _ => false
    };
}

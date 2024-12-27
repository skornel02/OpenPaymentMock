using OpenPaymentMock.Model.Enums;

namespace OpenPaymentMock.Model.Extensions;

public static class PaymentAttemptStatusExtensions
{
    public static bool IsFinalized(this PaymentAttemptStatus status) => status switch
    {
        PaymentAttemptStatus.Succeeded
            or PaymentAttemptStatus.TimedOut
            or PaymentAttemptStatus.PaymentError
            => true,
        _ => false
    };
}

namespace OpenPaymentMock.Model.Enums;
public enum PaymentAttemptStatus
{
    /// <summary>
    /// No payment attempt exists.
    /// </summary>
    NotAttempted,

    /// <summary>
    /// The payment attempt has been started.
    /// </summary>
    Started,

    /// <summary>
    /// The payment was a success.
    /// </summary>
    Succeeded,

    /// <summary>
    /// The payment attempt timed out.
    /// </summary>
    TimedOut,

    /// <summary>
    /// The payment attempt requires a bank verification.
    /// </summary>
    BankVerificationRequired,

    /// <summary>
    /// A payment error occurred.
    /// </summary>
    PaymentError,
}

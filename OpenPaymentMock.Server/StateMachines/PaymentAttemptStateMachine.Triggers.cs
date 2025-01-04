namespace OpenPaymentMock.Server.StateMachines;

public enum PaymentAttemptTrigger
{
    Started,
    Success,
    Issue,
    Cancel,
    Timeout,
    BankVerification,
}

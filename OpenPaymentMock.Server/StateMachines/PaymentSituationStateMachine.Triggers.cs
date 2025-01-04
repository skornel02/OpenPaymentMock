namespace OpenPaymentMock.Server.StateMachines;

public enum PaymentSituationTriggers
{
    Started,
    Failed,
    Cancel,
    Success,
    Refund,
}

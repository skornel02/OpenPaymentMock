using OpenPaymentMock.Model.Entities;
using OpenPaymentMock.Model.Enums;
using Stateless;

namespace OpenPaymentMock.Server.StateMachines;

public static class PaymentAttemptStateMachine
{
    public static StateMachine<PaymentAttemptStatus, PaymentAttemptTrigger> GetStateMachine(
        this PaymentAttemptEntity attempt,
        out StateMachine<PaymentAttemptStatus, PaymentAttemptTrigger>.TriggerWithParameters<string?> paymentIssueTrigger)
    {
        var stateMachine = new StateMachine<PaymentAttemptStatus, PaymentAttemptTrigger>(
            () => attempt.Status,
            s => attempt.Status = s
        );

        paymentIssueTrigger = stateMachine.SetTriggerParameters<string?>(PaymentAttemptTrigger.Issue);

        stateMachine.Configure(PaymentAttemptStatus.NotAttempted)
            .Permit(PaymentAttemptTrigger.Started, PaymentAttemptStatus.Started);

        stateMachine.Configure(PaymentAttemptStatus.Started)
            .OnEntry(() =>
            {
                attempt.PaymentSituation.GetStateMachine().Fire(PaymentSituationTriggers.Started);
            })
            .Permit(PaymentAttemptTrigger.Success, PaymentAttemptStatus.Succeeded)
            .Permit(PaymentAttemptTrigger.Cancel, PaymentAttemptStatus.PaymentError)
            .Permit(PaymentAttemptTrigger.Issue, PaymentAttemptStatus.PaymentError)
            .Permit(PaymentAttemptTrigger.Timeout, PaymentAttemptStatus.TimedOut)
            .Permit(PaymentAttemptTrigger.BankVerification, PaymentAttemptStatus.BankVerificationRequired);

        stateMachine.Configure(PaymentAttemptStatus.BankVerificationRequired)
            .Permit(PaymentAttemptTrigger.Success, PaymentAttemptStatus.Succeeded)
            .Permit(PaymentAttemptTrigger.Issue, PaymentAttemptStatus.PaymentError);

        stateMachine.Configure(PaymentAttemptStatus.Succeeded)
            .OnEntry(() =>
            {
                attempt.FinishedAt = DateTimeOffset.Now;

                attempt.PaymentSituation.GetStateMachine().Fire(PaymentSituationTriggers.Success);
            });

        stateMachine.Configure(PaymentAttemptStatus.PaymentError)
            .OnEntryFrom(paymentIssueTrigger, (issue) =>
            {
                attempt.PaymentError = issue;
            })
            .OnEntryFrom(PaymentAttemptTrigger.Cancel, () =>
            {
                attempt.PaymentError = "Payment was cancelled by the user";

                attempt.PaymentSituation.GetStateMachine().Fire(PaymentSituationTriggers.Failed);
            })
            .OnEntry(() =>
            {
                attempt.FinishedAt = DateTimeOffset.Now;
            });

        stateMachine.Configure(PaymentAttemptStatus.TimedOut)
            .OnEntry(() =>
            {
                attempt.FinishedAt = DateTimeOffset.Now;
                attempt.PaymentError = "Timed out!";
            });

        stateMachine.OnUnhandledTrigger((_, _) =>
        {
            throw new InvalidOperationException("Unhandled trigger!");
        });

        return stateMachine;
    }
}

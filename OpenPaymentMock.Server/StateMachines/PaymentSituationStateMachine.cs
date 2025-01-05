using MassTransit;
using OpenPaymentMock.Model.Entities;
using OpenPaymentMock.Model.Enums;
using Stateless;

namespace OpenPaymentMock.Server.StateMachines;

public static class PaymentSituationStateMachine
{

    public static StateMachine<PaymentSituationStatus, PaymentSituationTriggers> GetStateMachine(
        this PaymentSituationEntity situation)
    {
        var stateMachine = new StateMachine<PaymentSituationStatus, PaymentSituationTriggers>(
            () => situation.Status,
            s => situation.Status = s
        );

        stateMachine.Configure(PaymentSituationStatus.Created)
            .Permit(PaymentSituationTriggers.Started, PaymentSituationStatus.Processing)
            .Permit(PaymentSituationTriggers.Cancel, PaymentSituationStatus.Cancelled);

        stateMachine.Configure(PaymentSituationStatus.Processing)
            .PermitReentry(PaymentSituationTriggers.Started)
            .Permit(PaymentSituationTriggers.Success, PaymentSituationStatus.Succeeded)
            .Permit(PaymentSituationTriggers.Failed, PaymentSituationStatus.Failed)
            .Permit(PaymentSituationTriggers.Cancel, PaymentSituationStatus.Cancelled);

        stateMachine.Configure(PaymentSituationStatus.Succeeded)
            .Permit(PaymentSituationTriggers.Refund, PaymentSituationStatus.Refunded)
            .OnEntry(() =>
            {
                situation.FinishedAt = DateTimeOffset.Now;

                situation.Callback = new()
                {
                    Id = NewId.NextGuid(),
                    CallbackUrl = situation.CallbackUrl,
                    PaymentSituationId = situation.Id,
                    Status = PaymentCallbackStatus.Pending,
                };
            });

        stateMachine.Configure(PaymentSituationStatus.Failed)
            .OnEntry(() =>
            {
                situation.FinishedAt = DateTimeOffset.Now;

                situation.Callback = new()
                {
                    Id = NewId.NextGuid(),
                    CallbackUrl = situation.CallbackUrl,
                    PaymentSituationId = situation.Id,
                    Status = PaymentCallbackStatus.Pending,
                };
            });

        stateMachine.OnUnhandledTrigger((_, _) =>
        {
            throw new InvalidOperationException("Unhandled trigger!");
        });

        return stateMachine;
    }
}

﻿using OpenPaymentMock.Model.Enums;

namespace OpenPaymentMock.Model.Entities;

public class PaymentAttemptEntity
{
    public required Guid Id { get; set; }

    public required PaymentAttemptStatus Status { get; set; } = PaymentAttemptStatus.Started;

    public string? PaymentError { get; set; }

    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? FinishedAt { get; set; }

    public required Guid PaymentSituationId { get; set; }

    public PaymentSituationEntity PaymentSituation { get; set; } = null!;
}
﻿using OpenPaymentMock.Model.Enums;

namespace OpenPaymentMock.Communication.Payments;

public record CurrentPaymentAttemptDto(
    Guid Id,
    PaymentAttemptStatus Status,
    string? PaymentError,
    DateTimeOffset CreatedAt,
    DateTimeOffset? FinishedAt,
    Guid PaymentSituationId,
    PaymentSituationPublicDto PaymentSituation
);
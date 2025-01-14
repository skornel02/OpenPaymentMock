﻿using System.ComponentModel.DataAnnotations;

namespace OpenPaymentMock.Model.Entities;

public class PartnerAccessKeyEntity
{
    public required Guid Id { get; set; }

    public string Name { get; set; } = "-";

    public required string Key { get; set; } = null!;

    public required DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? ExpiresAt { get; set; }

    public DateTimeOffset? LastUsed { get; set; }

    public bool Deleted { get; set; }

    public long UsageCount { get; set; }

    public required Guid PartnerId { get; set; }

    public PartnerEntity Partner { get; set; } = null!;
}

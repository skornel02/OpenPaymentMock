namespace OpenPaymentMock.Communication.PartnerAccessKeys;

public record PartnerAccessKeyDetailsDto(
    Guid Id,
    string Name,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ExpiresAt,
    DateTimeOffset? LastUsed,
    bool Deleted,
    long UsageCount,
    string PartnerName,
    Guid PartnerId
);

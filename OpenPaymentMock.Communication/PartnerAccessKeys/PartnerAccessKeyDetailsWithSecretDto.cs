namespace OpenPaymentMock.Communication.PartnerAccessKeys;

public record PartnerAccessKeyDetailsWithSecretDto(
    Guid Id,
    string Key,
    string Name,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ExpiresAt,
    DateTimeOffset? LastUsed,
    bool Deleted,
    long UsageCount,
    string PartnerName,
    Guid PartnerId
);


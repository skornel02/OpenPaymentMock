namespace OpenPaymentMock.Communication.PartnerAccessKeys;

public record PartnerAccessKeyDetailsWithSecretDto(
    Guid Id,
    string Key,
    string Name,
    DateTime CreatedAt,
    DateTime? ExpiresAt,
    DateTime? LastUsed,
    bool Deleted,
    long UsageCount,
    string PartnerName,
    Guid PartnerId
);


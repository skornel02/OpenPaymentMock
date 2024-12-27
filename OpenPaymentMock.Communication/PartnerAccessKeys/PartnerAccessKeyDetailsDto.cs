namespace OpenPaymentMock.Communication.PartnerAccessKeys;

public record PartnerAccessKeyDetailsDto(
    Guid Id,
    string Name,
    DateTime CreatedAt,
    DateTime? ExpiresAt,
    DateTime? LastUsed,
    bool Deleted,
    long UsageCount,
    string PartnerName,
    Guid PartnerId
);

namespace OpenPaymentMock.Communication.PartnerAccessKeys;
public record PartnerAccessKeyCreationDto(
    string Name,
    DateTime? ExpiresAt
);

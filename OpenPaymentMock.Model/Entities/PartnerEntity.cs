namespace OpenPaymentMock.Model.Entities;

public class PartnerEntity
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public List<PaymentSituationEntity> PaymentSituations { get; set; } = null!;

    public List<PartnerAccessKeyEntity> AccessKeys { get; set; } = null!;
}

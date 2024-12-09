using MassTransit;
using OpenPaymentMock.Communication.Partners;
using OpenPaymentMock.Model.Entities;
using Riok.Mapperly.Abstractions;

namespace OpenPaymentMock.Server.Extensions;

[Mapper]
public static partial class ModelMapperExtensions
{   
    private static Guid GenerateId() => NewId.NextGuid();

    [MapperIgnoreSource(nameof(PartnerEntity.PaymentSituations))]
    public static partial IQueryable<PartnerShortDto> ToShortDto(this IQueryable<PartnerEntity> partners);

    [MapperIgnoreSource(nameof(PartnerEntity.PaymentSituations))]
    public static partial PartnerShortDto ToShortDto(this PartnerEntity entity);

    [MapValue(nameof(PartnerEntity.Id), Use = nameof(GenerateId))]
    public static partial PartnerEntity ToEntity(this PartnerCreationDto dto);
}

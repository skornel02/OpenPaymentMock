using MassTransit;
using OpenPaymentMock.Communication.PartnerAccessKeys;
using OpenPaymentMock.Communication.Partners;
using OpenPaymentMock.Communication.Payment;
using OpenPaymentMock.Communication.Payments;
using OpenPaymentMock.Model.Entities;
using OpenPaymentMock.Model.Enums;
using OpenPaymentMock.Server.Services;
using Riok.Mapperly.Abstractions;

namespace OpenPaymentMock.Server.Extensions;

[Mapper]
public static partial class ModelMapperExtensions
{
    private static Guid GenerateId() => NewId.NextGuid();

    private static DateTime Now() => DateTime.UtcNow;

    private static string GenerateAccessToken() => AccessKeyService.RandomAccessToken;

    public static partial IQueryable<PartnerShortDto> ToShortDto(this IQueryable<PartnerEntity> partners);

    [MapperIgnoreSource(nameof(PartnerEntity.PaymentSituations))]
    [MapperIgnoreSource(nameof(PartnerEntity.AccessKeys))]
    public static partial PartnerShortDto ToShortDto(this PartnerEntity partner);

    [MapValue(nameof(PartnerEntity.Id), Use = nameof(GenerateId))]
    [MapperIgnoreTarget(nameof(PartnerEntity.PaymentSituations))]
    [MapperIgnoreTarget(nameof(PartnerEntity.AccessKeys))]
    public static partial PartnerEntity ToEntity(this PartnerCreationDto dto);

    public static partial IQueryable<PaymentSituationDetailsDto> ToDetailedDto(this IQueryable<PaymentSituationEntity> situations);

    [MapperIgnoreSource(nameof(PaymentSituationEntity.Partner))]
    [MapperIgnoreSource(nameof(PaymentSituationEntity.PaymentAttempts))]
    public static partial PaymentSituationDetailsDto ToDetailedDto(this PaymentSituationEntity situation);


    [MapValue(nameof(PaymentSituationEntity.Id), Use = nameof(GenerateId))]
    [MapValue(nameof(PaymentSituationEntity.CreatedAt), Use = nameof(Now))]
    [MapValue(nameof(PaymentSituationEntity.Status), PaymentSituationStatus.Created)]
    [MapperIgnoreTarget(nameof(PaymentSituationEntity.Partner))]
    [MapperIgnoreTarget(nameof(PaymentSituationEntity.FinishedAt))]
    [MapperIgnoreTarget(nameof(PaymentSituationEntity.PaymentAttempts))]
    public static partial PaymentSituationEntity ToEntity(this PaymentSituationCreationDto dto, Guid partnerId);

    public static partial IQueryable<PaymentAttemptDetailsDto> ToDetailedDto(this IQueryable<PaymentAttemptEntity> situations);

    [MapperIgnoreSource(nameof(PaymentAttemptEntity.PaymentSituation))]
    public static partial PaymentAttemptDetailsDto ToDetailedDto(this PaymentAttemptEntity situation);

    public static partial IQueryable<PartnerAccessKeyDetailsDto> ToDetailedDto(this IQueryable<PartnerAccessKeyEntity> keys);

    [MapperIgnoreSource(nameof(PartnerAccessKeyEntity.Key))]
    public static partial PartnerAccessKeyDetailsDto ToDetailedDto(this PartnerAccessKeyEntity key);

    public static partial PartnerAccessKeyDetailsWithSecretDto ToDetailedWithKeyDto(this PartnerAccessKeyEntity key);

    [MapValue(nameof(PartnerAccessKeyEntity.Id), Use = nameof(GenerateId))]
    [MapValue(nameof(PartnerAccessKeyEntity.CreatedAt), Use = nameof(Now))]
    [MapValue(nameof(PartnerAccessKeyEntity.Key), Use = nameof(GenerateAccessToken))]
    [MapperIgnoreTarget(nameof(PartnerAccessKeyEntity.LastUsed))]
    [MapperIgnoreTarget(nameof(PartnerAccessKeyEntity.UsageCount))]
    [MapperIgnoreTarget(nameof(PartnerAccessKeyEntity.Deleted))]
    [MapperIgnoreTarget(nameof(PartnerAccessKeyEntity.Partner))]
    public static partial PartnerAccessKeyEntity ToEntity(this PartnerAccessKeyCreationDto dto, Guid partnerId);
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenPaymentMock.Model.Entities;

namespace OpenPaymentMock.Server.Persistance.Configurations;

public class PaymentSituationEntityConfiguration : IEntityTypeConfiguration<PaymentSituationEntity>
{
    public void Configure(EntityTypeBuilder<PaymentSituationEntity> builder)
    {
        builder.HasOne(_ => _.Partner)
            .WithMany(_ => _.PaymentSituations)
            .HasForeignKey(_ => _.PartnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(_ => _.PaymentOptions, _ =>
        {
            _.ToJson();
        });
    }
}

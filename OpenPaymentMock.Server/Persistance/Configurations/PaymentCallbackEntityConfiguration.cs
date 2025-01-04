using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenPaymentMock.Model.Entities;

namespace OpenPaymentMock.Server.Persistance.Configurations;

public class PaymentCallbackEntityConfiguration : IEntityTypeConfiguration<PaymentCallbackEntity>
{
    public void Configure(EntityTypeBuilder<PaymentCallbackEntity> builder)
    {
        builder.Property(_ => _.Id)
            .ValueGeneratedOnAdd();

        builder.HasOne(_ => _.PaymentSituation)
            .WithOne(_ => _.Callback)
            .HasForeignKey<PaymentCallbackEntity>(_ => _.PaymentSituationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

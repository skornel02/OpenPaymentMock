using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenPaymentMock.Model.Entities;

namespace OpenPaymentMock.Server.Persistance.Configurations;

public class PaymentCallbackEntityConfiguration : IEntityTypeConfiguration<PaymentCallbackEntity>
{
    public void Configure(EntityTypeBuilder<PaymentCallbackEntity> builder)
    {
        builder.HasOne(_ => _.PaymentSituation)
            .WithMany(_ => _.Callbacks)
            .HasForeignKey(_ => _.PaymentSituationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenPaymentMock.Model.Entities;

namespace OpenPaymentMock.Server.Persistance.Configurations;

public class PaymentAttemptEntityConfiguration : IEntityTypeConfiguration<PaymentAttemptEntity>
{
    public void Configure(EntityTypeBuilder<PaymentAttemptEntity> builder)
    {
        builder.HasOne(_ => _.PaymentSituation)
            .WithMany(_ => _.PaymentAttempts)
            .HasForeignKey(_ => _.PaymentSituationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

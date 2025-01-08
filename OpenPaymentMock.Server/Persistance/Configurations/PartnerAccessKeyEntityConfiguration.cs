using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenPaymentMock.Model.Entities;

namespace OpenPaymentMock.Server.Persistance.Configurations;

public class PartnerAccessKeyEntityConfiguration : IEntityTypeConfiguration<PartnerAccessKeyEntity>
{
    public void Configure(EntityTypeBuilder<PartnerAccessKeyEntity> builder)
    {
        builder.HasIndex(_ => _.Key)
            .IsUnique();

        builder.HasOne(_ => _.Partner)
            .WithMany(_ => _.AccessKeys)
            .HasForeignKey(_ => _.PartnerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

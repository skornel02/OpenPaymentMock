using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenPaymentMock.Model.Entities;

namespace OpenPaymentMock.Server.Persistance.Configurations;

public class PartnerEntityConfiguration : IEntityTypeConfiguration<PartnerEntity>
{
    public void Configure(EntityTypeBuilder<PartnerEntity> builder)
    {
        builder.HasIndex(_ => _.Name)
            .IsUnique();
    }
}

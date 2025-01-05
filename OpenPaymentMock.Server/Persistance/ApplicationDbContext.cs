using Microsoft.EntityFrameworkCore;
using OpenPaymentMock.Model.Entities;

namespace OpenPaymentMock.Server.Persistance;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<PartnerEntity> Partners { get; set; } = null!;

    public DbSet<PartnerAccessKeyEntity> PartnerAccessKeys { get; set; } = null!;
    public DbSet<PaymentAttemptEntity> PaymentAttempts { get; set; } = null!;
    public DbSet<PaymentSituationEntity> PaymentSituations { get; set; } = null!;

    public DbSet<PaymentCallbackEntity> Callbacks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder
            .EnableSensitiveDataLogging();
#endif

        base.OnConfiguring(optionsBuilder);
    }
}

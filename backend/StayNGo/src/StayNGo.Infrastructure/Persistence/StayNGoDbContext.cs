using Microsoft.EntityFrameworkCore;
using StayNGo.Domain.Entities;
using StayNGo.Domain.Interfaces;
using StayNGo.Domain.ValueObjects;
using StayNGo.Infrastructure.Persistence.Converters;

namespace StayNGo.Infrastructure.Persistence;

public class StayNGoDbContext(DbContextOptions<StayNGoDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Listing> Listings { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("btree_gist");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StayNGoDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<DateRange>()
            .HaveColumnType("daterange")
            .HaveConversion<DateRangeConverter>();

        builder.ComplexProperties<Money>();
    }
}
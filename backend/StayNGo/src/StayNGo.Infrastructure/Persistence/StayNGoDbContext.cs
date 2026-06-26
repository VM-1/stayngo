using Microsoft.EntityFrameworkCore;
using Npgsql;
using StayNGo.Domain.Entities;
using StayNGo.Domain.Exceptions;
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

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
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

        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            if ((ex.InnerException as PostgresException)?.SqlState.Contains("23P01") == true)
            {
                throw new DomainException(
                    "Action cannot be performed because a record with the same data already exists.");
            }

            throw;
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.ComplexProperties<Money>();

        builder.Properties<IanaTimeZone>()
            .HaveConversion<IanaTimeZoneConverter>();
    }
}
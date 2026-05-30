using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StayNGo.Domain.Entities;

namespace StayNGo.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : BaseEntityTypeConfiguration<Booking>
{
    public override void Configure(EntityTypeBuilder<Booking> builder)
    {
        base.Configure(builder);
        
        // No-double-booking invariant: Postgres EXCLUDE USING gist constraint
        // `bookings_no_overlap_confirmed` is added via raw SQL in the
        // InitialSchema migration. EF Core has no native API for EXCLUDE.
        // Do not rely on application code to enforce overlap.
    }
}
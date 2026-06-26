using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StayNGo.Domain.Entities;

namespace StayNGo.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : BaseEntityTypeConfiguration<Booking>
{
    public override void Configure(EntityTypeBuilder<Booking> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => new { x.ListingId, x.CheckIn, x.CheckOut });
        builder.HasIndex(x => new { x.GuestUserId, x.IdempotencyKey }).IsUnique();
    }
}
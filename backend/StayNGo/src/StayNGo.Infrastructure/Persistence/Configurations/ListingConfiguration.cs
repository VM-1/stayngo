using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StayNGo.Domain.Entities;

namespace StayNGo.Infrastructure.Persistence.Configurations;

public class ListingConfiguration : BaseEntityTypeConfiguration<Listing>
{
    public override void Configure(EntityTypeBuilder<Listing> builder)
    {
        base.Configure(builder);
        
        builder.HasMany(x => x.Bookings)
            .WithOne(x => x.Listing)
            .HasForeignKey(x => x.ListingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
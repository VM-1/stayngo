using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StayNGo.Domain.Entities;

namespace StayNGo.Infrastructure.Persistence.Configurations;

public class UserConfiguration : BaseEntityTypeConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.ClerkId).IsUnique();
        builder.Property(x => x.Email).HasColumnType("citext");

        builder.Property(x => x.DisplayName).HasMaxLength(50).IsRequired();
        
        builder.HasMany(x => x.Listings)
            .WithOne(x => x.Owner)
            .HasForeignKey(x => x.OwnerUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(x => x.Bookings)
            .WithOne(x => x.Guest)
            .HasForeignKey(x => x.GuestUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
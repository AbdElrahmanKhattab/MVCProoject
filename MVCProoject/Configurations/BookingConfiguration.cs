using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.Models;

namespace MVC.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.Property(x => x.BookingDate)
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(x => x.Member)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.GymSession)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.GymSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.MemberId, x.GymSessionId }).IsUnique();
        }
    }
}

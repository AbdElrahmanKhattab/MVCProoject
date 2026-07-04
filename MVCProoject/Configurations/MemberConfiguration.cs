using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.Models;

namespace MVC.Configurations
{
    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Email).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Phone).HasMaxLength(30).IsRequired();
            builder.Property(x => x.BuildingNumber).HasMaxLength(20).IsRequired();
            builder.Property(x => x.Street).HasMaxLength(100).IsRequired();
            builder.Property(x => x.City).HasMaxLength(60).IsRequired();
            builder.Property(x => x.PhotoFileName).HasMaxLength(255);
            builder.Property(x => x.PhotoContentType).HasMaxLength(100);
            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasOne(x => x.HealthRecord)
                .WithOne(x => x.Member)
                .HasForeignKey<HealthRecord>(x => x.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

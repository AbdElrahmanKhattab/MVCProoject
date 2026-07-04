using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.Models;

namespace MVC.Configurations
{
    public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
    {
        public void Configure(EntityTypeBuilder<Membership> builder)
        {
            builder.Property(x => x.StartDate)
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(x => x.Member)
                .WithMany(x => x.Memberships)
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Plan)
                .WithMany(x => x.Memberships)
                .HasForeignKey(x => x.PlanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

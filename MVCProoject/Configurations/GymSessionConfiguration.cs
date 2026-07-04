using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.Models;

namespace MVC.Configurations
{
    public class GymSessionConfiguration : IEntityTypeConfiguration<GymSession>
    {
        public void Configure(EntityTypeBuilder<GymSession> builder)
        {
            builder.Property(x => x.CategoryName)
                .HasColumnType("varchar")
                .HasMaxLength(20);

            builder.Property(x => x.TrainerName)
                .HasColumnType("varchar")
                .HasMaxLength(50);

            builder.Property(x => x.Description)
                .HasColumnType("varchar")
                .HasMaxLength(200);

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("SessionCapacityCheck", "Capacity BETWEEN 1 AND 25");
                tb.HasCheckConstraint("SessionDateCheck", "EndDate > StartDate");
            });
        }
    }
}

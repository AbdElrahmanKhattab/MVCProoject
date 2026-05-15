using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.Models;

namespace MVC.Configurations
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(x => x.Name)
                .HasColumnType("varchar")
                .HasMaxLength(50);

            builder.Property(x => x.Description)
                .HasMaxLength(200);

            builder.Property(x => x.Price)
                .HasPrecision(10, 2);

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("PlanDurationCheck",
                    "DurationDays Between 1 and 365");
            });

            builder.HasData(
                new Plan
                {
                    Id = 1,
                    Name = "Basic",
                    Description = "Entry membership for gym access and starter workouts.",
                    DurationDays = 30,
                    Price = 300,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1)
                },
                new Plan
                {
                    Id = 2,
                    Name = "Standard",
                    Description = "Balanced plan for regular members with group class access.",
                    DurationDays = 60,
                    Price = 600,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1)
                },
                new Plan
                {
                    Id = 3,
                    Name = "Premium",
                    Description = "Full access plan with premium class support.",
                    DurationDays = 90,
                    Price = 900,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1)
                },
                new Plan
                {
                    Id = 4,
                    Name = "Annual",
                    Description = "Best value yearly membership for committed members.",
                    DurationDays = 365,
                    Price = 3000,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1)
                });
        }
    }
}

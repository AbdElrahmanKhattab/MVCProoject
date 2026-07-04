using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.Models;

namespace MVC.Configurations
{
    public class HealthRecordConfiguration : IEntityTypeConfiguration<HealthRecord>
    {
        public void Configure(EntityTypeBuilder<HealthRecord> builder)
        {
            builder.Property(x => x.Height).HasPrecision(5, 2);
            builder.Property(x => x.Weight).HasPrecision(5, 2);
            builder.Property(x => x.BloodType).HasMaxLength(5).IsRequired();
            builder.Property(x => x.Note).HasMaxLength(500);
        }
    }
}

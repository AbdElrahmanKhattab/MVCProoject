using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.Models;

namespace MVC.Configurations
{
    public class HealthRecordConfiguration : IEntityTypeConfiguration<HealthRecord>
    {
        public void Configure(EntityTypeBuilder<HealthRecord> builder)
        {
            builder.Property(x => x.Height).HasPrecision(6, 2);
            builder.Property(x => x.Weight).HasPrecision(6, 2);
            builder.Property(x => x.BloodType).HasColumnType("varchar").HasMaxLength(3);
            builder.Property(x => x.Note).HasMaxLength(250);
            builder.Property(x => x.LastUpdate).HasDefaultValueSql("GETDATE()");
        }
    }
}

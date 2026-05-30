using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.Models;

namespace MVC.Configurations
{
    public class TrainerConfiguration : IEntityTypeConfiguration<Trainer>
    {
        public void Configure(EntityTypeBuilder<Trainer> builder)
        {
            builder.Property(x => x.Name).HasColumnType("varchar").HasMaxLength(50);
            builder.Property(x => x.Email).HasColumnType("varchar").HasMaxLength(100);
            builder.Property(x => x.Phone).HasColumnType("varchar").HasMaxLength(11);
            builder.Property(x => x.Street).HasColumnType("varchar").HasMaxLength(150);
            builder.Property(x => x.City).HasColumnType("varchar").HasMaxLength(100);
            builder.Property(x => x.HireDate).HasDefaultValueSql("GETDATE()");
            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.Phone).IsUnique();
        }
    }
}

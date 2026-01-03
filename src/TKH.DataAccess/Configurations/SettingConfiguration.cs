using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKH.Entities;

namespace TKH.DataAccess.Configurations
{
    public class SettingConfiguration : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.ToTable("Settings");

            builder.HasKey(setting => setting.Id);

            builder.Property(setting => setting.Name).IsRequired().HasMaxLength(200);

            builder.Property(setting => setting.Value).IsRequired().HasMaxLength(int.MaxValue);

            builder.HasIndex(setting => setting.Name).IsUnique();
        }
    }
}

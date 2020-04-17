using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Foundation.Persistance.EntityConfiguration
{
    public class UserSettingConfiguration : IEntityTypeConfiguration<UserSetting>
    {
        private string _tenantId;

        public UserSettingConfiguration(string tenantId)
        {
            _tenantId = tenantId;
        }

        public void Configure(EntityTypeBuilder<UserSetting> builder)
        {
            builder.ToTable("UserSetting");

            builder.Property(u => u.TenantId).IsRequired();
            builder.Property(u => u.Id).ValueGeneratedOnAdd();
            builder.Property(u => u.Status).IsRequired();
            builder.Property(u => u.Value).IsRequired();

            builder.HasOne(u => u.SettingHeader)
                .WithMany(p => p.UserSettings)
                .HasForeignKey(u => u.SettingHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(u => u.TenantId == Guid.Parse(_tenantId));
        }
    }
}

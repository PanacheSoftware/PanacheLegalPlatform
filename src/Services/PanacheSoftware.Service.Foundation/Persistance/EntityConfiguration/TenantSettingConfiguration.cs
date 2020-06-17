using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Foundation.Persistance.EntityConfiguration
{
    public class TenantSettingConfiguration : IEntityTypeConfiguration<TenantSetting>
    {
        public TenantSettingConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<TenantSetting> builder)
        {
            builder.ToTable("TenantSetting");

            builder.Property(t => t.TenantId).IsRequired();
            builder.Property(t => t.Id).ValueGeneratedOnAdd();
            builder.Property(t => t.Status).IsRequired();
            builder.Property(t => t.Value).IsRequired();

            builder.HasOne(s => s.SettingHeader)
                .WithMany(t => t.TenantSettings)
                .HasForeignKey(t => t.SettingHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

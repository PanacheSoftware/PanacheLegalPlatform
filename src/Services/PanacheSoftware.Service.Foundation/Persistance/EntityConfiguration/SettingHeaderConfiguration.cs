using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Foundation.Persistance.EntityConfiguration
{
    public class SettingHeaderConfiguration : IEntityTypeConfiguration<SettingHeader>
    {
        public SettingHeaderConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<SettingHeader> builder)
        {
            builder.ToTable("SettingHeader");

            builder.Property(p => p.TenantId).IsRequired();
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.Status).IsRequired();
            builder.Property(p => p.DefaultValue).IsRequired();
            builder.Property(P => P.Value).IsRequired();
        }
    }
}

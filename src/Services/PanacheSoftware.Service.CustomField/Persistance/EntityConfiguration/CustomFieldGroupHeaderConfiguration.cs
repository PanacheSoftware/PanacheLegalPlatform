using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.CustomField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.CustomField.Persistance.EntityConfiguration
{
    public class CustomFieldGroupHeaderConfiguration : IEntityTypeConfiguration<CustomFieldGroupHeader>
    {
        public CustomFieldGroupHeaderConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<CustomFieldGroupHeader> builder)
        {
            builder.ToTable("CustomFieldGroupHeader");

            builder.Property(c => c.TenantId).IsRequired();
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Status).IsRequired();
            builder.Property(c => c.ShortName).IsRequired();
            builder.Property(c => c.LongName).IsRequired();
            builder.Property(c => c.Description).IsRequired();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.CustomField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.CustomField.Persistance.EntityConfiguration
{
    public class CustomFieldValueConfiguration : IEntityTypeConfiguration<CustomFieldValue>
    {
        public CustomFieldValueConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<CustomFieldValue> builder)
        {
            builder.ToTable("CustomFieldValue");

            builder.Property(c => c.TenantId).IsRequired();
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Status).IsRequired();
            builder.Property(c => c.LinkType).IsRequired();
            builder.Property(c => c.LinkId).IsRequired();
            builder.Property(c => c.FieldValue).IsRequired();
            //builder.HasKey(k => new { k.Id, k.CustomFieldHeaderId, k.LinkId });
        }
    }
}

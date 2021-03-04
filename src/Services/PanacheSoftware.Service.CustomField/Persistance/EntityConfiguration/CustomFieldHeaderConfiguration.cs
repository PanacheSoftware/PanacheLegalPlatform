using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.CustomField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.CustomField.Persistance.EntityConfiguration
{
    public class CustomFieldHeaderConfiguration : IEntityTypeConfiguration<CustomFieldHeader>
    {
        public CustomFieldHeaderConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<CustomFieldHeader> builder)
        {
            builder.ToTable("CustomFieldHeader");

            builder.Property(c => c.TenantId).IsRequired();
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Status).IsRequired();
            builder.Property(c => c.GDPR).IsRequired();
            builder.Property(c => c.History).IsRequired();
            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.Description).IsRequired();
            builder.Property(c => c.CustomFieldType).IsRequired();

            builder.HasOne(c => c.CustomFieldGroupHeader)
                .WithMany(c => c.CustomFieldHeaders)
                .HasForeignKey(c => c.CustomFieldGroupHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

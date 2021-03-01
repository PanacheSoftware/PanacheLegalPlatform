using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.CustomField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.CustomField.Persistance.EntityConfiguration
{
    public class CustomFieldDetailConfiguration : IEntityTypeConfiguration<CustomFieldDetail>
    {
        public CustomFieldDetailConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<CustomFieldDetail> builder)
        {
            builder.ToTable("CustomFieldDetail");

            builder.Property(c => c.TenantId).IsRequired();
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Status).IsRequired();

            builder.HasOne(c => c.CustomFieldHeader)
                .WithOne(h => h.CustomFieldDetail)
                .HasForeignKey<CustomFieldDetail>(c => c.CustomFieldHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

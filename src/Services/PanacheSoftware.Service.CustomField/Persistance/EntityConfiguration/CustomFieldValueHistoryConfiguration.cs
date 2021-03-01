using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.CustomField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.CustomField.Persistance.EntityConfiguration
{
    public class CustomFieldValueHistoryConfiguration : IEntityTypeConfiguration<CustomFieldValueHistory>
    {
        public CustomFieldValueHistoryConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<CustomFieldValueHistory> builder)
        {
            builder.ToTable("CustomFieldValueHistory");

            builder.Property(c => c.TenantId).IsRequired();
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Status).IsRequired();
            builder.Property(c => c.FieldValue).IsRequired();
            builder.Property(c => c.LinkType).IsRequired();
            builder.Property(c => c.LinkId).IsRequired();
            builder.Property(c => c.OriginalCreationDate).IsRequired();
            builder.Property(c => c.SequenceNo).IsRequired();

            builder.HasOne(c => c.CustomFieldValue)
                .WithMany(v => v.CustomFieldValueHistorys)
                .HasForeignKey(c => c.CustomFieldValueId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

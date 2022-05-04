using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.CustomField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.CustomField.Persistance.EntityConfiguration
{
    public class CustomFieldGroupLinkConfiguration : IEntityTypeConfiguration<CustomFieldGroupLink>
    {
        public CustomFieldGroupLinkConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<CustomFieldGroupLink> builder)
        {
            builder.ToTable("CustomFieldGroupLink");

            builder.Property(l => l.TenantId).IsRequired();
            builder.Property(l => l.Id).ValueGeneratedOnAdd();

            builder.Property(l => l.Status).IsRequired();

            builder.Property(l => l.LinkId).IsRequired();
            builder.Property(l => l.LinkType).IsRequired();
            builder.Property(l => l.CustomFieldGroupHeaderId).IsRequired();

            //builder.HasOne(l => l.FileHeader)
            //    .WithMany(h => h.FileLinks)
            //    .HasForeignKey(l => l.FileHeaderId)
            //    .IsRequired()
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

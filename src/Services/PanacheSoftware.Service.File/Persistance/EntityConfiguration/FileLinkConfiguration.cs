using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.File.Persistance.EntityConfiguration
{
    public class FileLinkConfiguration : IEntityTypeConfiguration<FileLink>
    {
        private string _tenantId;

        public FileLinkConfiguration(string tenantId)
        {
            _tenantId = tenantId;
        }

        public void Configure(EntityTypeBuilder<FileLink> builder)
        {
            builder.ToTable("FileLink");

            builder.Property(l => l.TenantId).IsRequired();
            builder.Property(l => l.Id).ValueGeneratedOnAdd();

            builder.Property(l => l.Status).IsRequired();

            builder.Property(l => l.LinkId).IsRequired();
            builder.Property(l => l.LinkType).IsRequired();
            builder.Property(l => l.FileHeaderId).IsRequired();

            //builder.HasOne(l => l.FileHeader)
            //    .WithMany(h => h.FileLinks)
            //    .HasForeignKey(l => l.FileHeaderId)
            //    .IsRequired()
            //    .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(l => l.TenantId == Guid.Parse(_tenantId));
        }
    }
}

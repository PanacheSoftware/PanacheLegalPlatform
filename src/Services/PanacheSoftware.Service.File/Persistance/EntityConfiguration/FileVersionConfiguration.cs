using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.File.Persistance.EntityConfiguration
{
    public class FileVersionConfiguration : IEntityTypeConfiguration<FileVersion>
    {
        private string _tenantId;

        public FileVersionConfiguration(string tenantId)
        {
            _tenantId = tenantId;
        }
        public void Configure(EntityTypeBuilder<FileVersion> builder)
        {
            builder.ToTable("FileVersion");

            builder.Property(v => v.TenantId).IsRequired();
            builder.Property(v => v.Id).ValueGeneratedOnAdd();
            builder.Property(v => v.Status).IsRequired();

            //builder.HasOne(h => h.FileHeader)
            //    .WithMany(v => v.FileVersions)
            //    .HasForeignKey(h => h.FileHeaderId)
            //    .IsRequired()
            //    .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(c => c.TenantId == Guid.Parse(_tenantId));
        }
    }
}

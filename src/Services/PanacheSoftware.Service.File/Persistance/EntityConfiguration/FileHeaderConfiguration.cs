using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.File.Persistance.EntityConfiguration
{
    public class FileHeaderConfiguration : IEntityTypeConfiguration<FileHeader>
    {
        public FileHeaderConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<FileHeader> builder)
        {
            builder.ToTable("FileHeader");

            builder.Property(f => f.TenantId).IsRequired();
            builder.Property(f => f.Id).ValueGeneratedOnAdd();

            builder.Property(f => f.Status).IsRequired();

            builder.HasMany(h => h.FileVersions)
                .WithOne(v => v.FileHeader)
                .HasForeignKey(v => v.FileHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(h => h.FileLinks)
                .WithOne(l => l.FileHeader)
                .HasForeignKey(l => l.FileHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(h => h.FileDetail)
                .WithOne(d => d.FileHeader)
                .HasForeignKey<FileDetail>(d => d.FileHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

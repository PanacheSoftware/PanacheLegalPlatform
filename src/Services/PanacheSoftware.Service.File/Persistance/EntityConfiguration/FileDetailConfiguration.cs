using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.File.Persistance.EntityConfiguration
{
    public class FileDetailConfiguration : IEntityTypeConfiguration<FileDetail>
    {
        public FileDetailConfiguration()
        {
        }
        public void Configure(EntityTypeBuilder<FileDetail> builder)
        {
            builder.ToTable("FileDetail");

            builder.Property(d => d.TenantId).IsRequired();
            builder.Property(d => d.Id).ValueGeneratedOnAdd();
            builder.Property(d => d.Status).IsRequired();

            //builder.HasOne(d => d.FileHeader)
            //    .WithOne(d => d.FileDetail)
            //    .HasForeignKey<FileDetail>(h => h.FileHeaderId)
            //    .IsRequired()
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

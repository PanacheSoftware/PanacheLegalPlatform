using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Task.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Persistance.EntityConfiguration
{
    public class TemplateDetailConfiguration : IEntityTypeConfiguration<TemplateDetail>
    {
        public TemplateDetailConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<TemplateDetail> builder)
        {
            builder.ToTable("TemplateDetail");

            builder.Property(d => d.TenantId).IsRequired();
            builder.Property(d => d.Id).ValueGeneratedOnAdd();
            builder.Property(d => d.Status).IsRequired();

            builder.HasOne(d => d.TemplateHeader)
                .WithOne(h => h.TemplateDetail)
                .HasForeignKey<TemplateDetail>(d => d.TemplateHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

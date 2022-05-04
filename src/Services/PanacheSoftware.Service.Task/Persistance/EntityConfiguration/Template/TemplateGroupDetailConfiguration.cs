using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Task.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Persistance.EntityConfiguration
{
    public class TemplateGroupDetailConfiguration : IEntityTypeConfiguration<TemplateGroupDetail>
    {
        public TemplateGroupDetailConfiguration()
        {
        }
        public void Configure(EntityTypeBuilder<TemplateGroupDetail> builder)
        {
            builder.ToTable("TemplateGroupDetail");

            builder.Property(d => d.TenantId).IsRequired();
            builder.Property(d => d.Id).ValueGeneratedOnAdd();
            builder.Property(d => d.Status).IsRequired();

            builder.HasOne(d => d.TemplateGroupHeader)
                .WithOne(h => h.TemplateGroupDetail)
                .HasForeignKey<TemplateGroupDetail>(d => d.TemplateGroupHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Task.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Persistance.EntityConfiguration
{
    public class TemplateItemDetailConfiguration : IEntityTypeConfiguration<TemplateItemDetail>
    {
        public TemplateItemDetailConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<TemplateItemDetail> builder)
        {
            builder.ToTable("TemplateItemDetail");

            builder.Property(d => d.TenantId).IsRequired();
            builder.Property(d => d.Id).ValueGeneratedOnAdd();
            builder.Property(d => d.Status).IsRequired();

            builder.HasOne(d => d.TemplateItemHeader)
                .WithOne(h => h.TemplateItemDetail)
                .HasForeignKey<TemplateItemDetail>(d => d.TemplateItemHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

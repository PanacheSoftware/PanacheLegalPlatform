using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Task.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Persistance.EntityConfiguration
{
    public class TemplateItemHeaderConfiguration : IEntityTypeConfiguration<TemplateItemHeader>
    {
        public TemplateItemHeaderConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<TemplateItemHeader> builder)
        {
            builder.ToTable("TemplateItemHeader");

            builder.Property(d => d.TenantId).IsRequired();
            builder.Property(d => d.Id).ValueGeneratedOnAdd();
            builder.Property(d => d.Status).IsRequired();
            builder.Property(d => d.ShortName).IsRequired();

            builder.HasOne(c => c.TemplateGroupHeader)
                .WithMany(h => h.TemplateItemHeaders)
                .HasForeignKey(c => c.TemplateGroupHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

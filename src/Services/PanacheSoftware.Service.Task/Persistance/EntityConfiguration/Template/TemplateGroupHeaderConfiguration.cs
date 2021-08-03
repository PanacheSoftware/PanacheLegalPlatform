using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Task.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Persistance.EntityConfiguration
{
    public class TemplateGroupHeaderConfiguration : IEntityTypeConfiguration<TemplateGroupHeader>
    {
        public TemplateGroupHeaderConfiguration()
        {
        }
        public void Configure(EntityTypeBuilder<TemplateGroupHeader> builder)
        {
            builder.ToTable("TemplateGroupHeader");

            builder.Property(t => t.TenantId).IsRequired();
            builder.Property(t => t.Id).ValueGeneratedOnAdd();

            builder.Property(t => t.Status).IsRequired();
            builder.Property(t => t.ShortName).IsRequired();

            builder.HasOne(t => t.TemplateHeader)
                .WithMany(t => t.TemplateGroupHeaders)
                .HasForeignKey(t => t.TemplateHeaderId);
        }
    }
}

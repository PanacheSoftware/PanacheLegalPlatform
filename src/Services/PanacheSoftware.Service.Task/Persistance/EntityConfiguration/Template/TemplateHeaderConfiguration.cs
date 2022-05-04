using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Task.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Persistance.EntityConfiguration
{
    public class TemplateHeaderConfiguration : IEntityTypeConfiguration<TemplateHeader>
    {
        public TemplateHeaderConfiguration()
        {
        }
        public void Configure(EntityTypeBuilder<TemplateHeader> builder)
        {
            builder.ToTable("TemplateHeader");

            builder.Property(t => t.TenantId).IsRequired();
            builder.Property(t => t.Id).ValueGeneratedOnAdd();

            builder.Property(t => t.Status).IsRequired();
            builder.Property(t => t.ShortName).IsRequired();

            builder.HasMany(h => h.TemplateGroupHeaders)
                .WithOne(g => g.TemplateHeader)
                .HasForeignKey(g => g.TemplateHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(h => h.TemplateDetail)
                .WithOne(d => d.TemplateHeader)
                .HasForeignKey<TemplateDetail>(d => d.TemplateHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

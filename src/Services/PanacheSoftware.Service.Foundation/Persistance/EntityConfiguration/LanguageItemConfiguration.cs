using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Client;
using PanacheSoftware.Core.Domain.Language;
using System;

namespace PanacheSoftware.Service.Foundation.Persistance.EntityConfiguration
{
    public class LanguageItemConfiguration : IEntityTypeConfiguration<LanguageItem>
    {
        private string _tenantId;

        public LanguageItemConfiguration(string tenantId)
        {
            _tenantId = tenantId;
        }

        public void Configure(EntityTypeBuilder<LanguageItem> builder)
        {
            builder.ToTable("LanguageItem");

            builder.Property(l => l.TenantId).IsRequired();
            builder.Property(l => l.Id).ValueGeneratedOnAdd();
            builder.Property(l => l.Status).IsRequired();
            builder.Property(l => l.LanguageCodeId).IsRequired();
            builder.Property(l => l.Text).IsRequired();

            builder.HasOne(l => l.LanguageHeader)
                .WithMany(h => h.LanguageItems)
                .HasForeignKey(l => l.LanguageHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(l => l.TenantId == Guid.Parse(_tenantId));
        }
    }
}

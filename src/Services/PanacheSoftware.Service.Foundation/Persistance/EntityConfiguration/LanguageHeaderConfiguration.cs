using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Client;
using PanacheSoftware.Core.Domain.Language;
using System;

namespace PanacheSoftware.Service.Foundation.Persistance.EntityConfiguration
{
    public class LanguageHeaderConfiguration : IEntityTypeConfiguration<LanguageHeader>
    {
        private string _tenantId;

        public LanguageHeaderConfiguration(string tenantId)
        {
            _tenantId = tenantId;
        }

        public void Configure(EntityTypeBuilder<LanguageHeader> builder)
        {
            builder.ToTable("LanguageHeader");

            builder.Property(l => l.TenantId).IsRequired();
            builder.Property(l => l.Id).ValueGeneratedOnAdd();
            builder.Property(l => l.Status).IsRequired();
            builder.Property(l => l.TextCode).IsRequired();
            builder.Property(l => l.Text).IsRequired();

            builder.HasQueryFilter(l => l.TenantId == Guid.Parse(_tenantId));
        }
    }
}

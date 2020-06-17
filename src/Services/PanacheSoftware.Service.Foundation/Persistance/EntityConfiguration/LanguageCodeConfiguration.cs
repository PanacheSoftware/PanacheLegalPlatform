using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Language;
using System;

namespace PanacheSoftware.Service.Foundation.Persistance.EntityConfiguration
{
    public class LanguageCodeConfiguration : IEntityTypeConfiguration<LanguageCode>
    {
        public LanguageCodeConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<LanguageCode> builder)
        {
            builder.ToTable("LanguageCode");

            builder.Property(l => l.TenantId).IsRequired();
            builder.Property(l => l.Id).ValueGeneratedOnAdd();
            builder.Property(l => l.Status).IsRequired();
            builder.Property(l => l.LanguageCodeId).IsRequired();
        }
    }
}

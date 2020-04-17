using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Identity;
using System;

namespace PanacheSoftware.Identity.Data
{
    public class IdentityTenantsConfiguration : IEntityTypeConfiguration<IdentityTenant>
    {
        public IdentityTenantsConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<IdentityTenant> builder)
        {
            builder.ToTable("IdentityTenants");

            builder.Property(i => i.Id).ValueGeneratedOnAdd();
            builder.Property(i => i.Status).IsRequired();
            builder.Property(i => i.Domain).IsRequired();
            builder.Property(i => i.CreatedByEmail).IsRequired();
        }
    }
}

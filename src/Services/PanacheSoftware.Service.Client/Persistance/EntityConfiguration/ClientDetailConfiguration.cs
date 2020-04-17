using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Client;
using System;

namespace PanacheSoftware.Service.Client.Persistance.EntityConfiguration
{
    public class ClientDetailConfiguration : IEntityTypeConfiguration<ClientDetail>
    {
        private string _tenantId;

        public ClientDetailConfiguration(string tenantId)
        {
            _tenantId = tenantId;
        }
        public void Configure(EntityTypeBuilder<ClientDetail> builder)
        {
            builder.ToTable("ClientDetail");

            builder.Property(c => c.TenantId).IsRequired();
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Status).IsRequired();

            builder.HasOne(c => c.ClientHeader)
                .WithOne(h => h.ClientDetail)
                .HasForeignKey<ClientDetail>(c => c.ClientHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(c => c.TenantId == Guid.Parse(_tenantId));
        }
    }
}

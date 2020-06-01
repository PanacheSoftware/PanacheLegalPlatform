using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Client;
using System;

namespace PanacheSoftware.Service.Client.Persistance.EntityConfiguration
{
    public class ClientHeaderConfiguration : IEntityTypeConfiguration<ClientHeader>
    {

        public ClientHeaderConfiguration()
        {

        }

        public void Configure(EntityTypeBuilder<ClientHeader> builder)
        {
            builder.ToTable("ClientHeader");

            builder.Property(c => c.TenantId).IsRequired();
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Status).IsRequired();
            builder.Property(c => c.ShortName).IsRequired();
            builder.Property(c => c.LongName).IsRequired();
        }
    }
}

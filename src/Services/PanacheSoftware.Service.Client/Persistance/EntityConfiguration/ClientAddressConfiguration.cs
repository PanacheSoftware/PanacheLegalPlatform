﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Client;
using System;

namespace PanacheSoftware.Service.Client.Persistance.EntityConfiguration
{
    public class ClientAddressConfiguration : IEntityTypeConfiguration<ClientAddress>
    {
        private string _tenantId;

        public ClientAddressConfiguration(string tenantId)
        {
            _tenantId = tenantId;
        }

        public void Configure(EntityTypeBuilder<ClientAddress> builder)
        {
            builder.ToTable("ClientAddress");

            builder.Property(a => a.TenantId).IsRequired();
            builder.Property(a => a.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.Status).IsRequired();
            builder.Property(a => a.AddressType).IsRequired();

            builder.HasOne(a => a.ClientContact)
                .WithMany(h => h.ClientAddresses)
                .HasForeignKey(c => c.ClientContactId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(c => c.TenantId == Guid.Parse(_tenantId));
        }
    }
}

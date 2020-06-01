using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Client;
using System;

namespace PanacheSoftware.Service.Client.Persistance.EntityConfiguration
{
    public class ClientContactConfiguration : IEntityTypeConfiguration<ClientContact>
    {
        public ClientContactConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<ClientContact> builder)
        {
            builder.ToTable("ClientContact");

            builder.Property(c => c.TenantId).IsRequired();
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Status).IsRequired();

            builder.HasOne(c => c.ClientHeader)
                .WithMany(h => h.ClientContacts)
                .HasForeignKey(c => c.ClientHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

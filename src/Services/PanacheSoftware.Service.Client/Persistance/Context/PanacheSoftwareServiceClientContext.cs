using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Client;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Client.Persistance.EntityConfiguration;
using System;

namespace PanacheSoftware.Service.Client.Persistance.Context
{
    public class PanacheSoftwareServiceClientContext : DbContext
    {
        private readonly IUserProvider _userProvider;

        public PanacheSoftwareServiceClientContext(DbContextOptions<PanacheSoftwareServiceClientContext> options, IUserProvider userProvider) : base(options)
        {
            _userProvider = userProvider;
        }

        public virtual DbSet<ClientHeader> ClientHeaders { get; set; }
        public virtual DbSet<ClientDetail> ClientDetails { get; set; }
        public virtual DbSet<ClientContact> ClientContacts { get; set; }
        public virtual DbSet<ClientAddress> ClientAddresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ClientHeaderConfiguration());
            modelBuilder.ApplyConfiguration(new ClientDetailConfiguration());
            modelBuilder.ApplyConfiguration(new ClientContactConfiguration());
            modelBuilder.ApplyConfiguration(new ClientAddressConfiguration());

            modelBuilder.Entity<ClientHeader>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<ClientDetail>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<ClientContact>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<ClientAddress>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
        }
    }
}

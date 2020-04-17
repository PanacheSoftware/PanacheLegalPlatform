using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Client;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Client.Persistance.EntityConfiguration;

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
            var tenantId = _userProvider.GetTenantId();

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ClientHeaderConfiguration(tenantId));
            modelBuilder.ApplyConfiguration(new ClientDetailConfiguration(tenantId));
            modelBuilder.ApplyConfiguration(new ClientContactConfiguration(tenantId));
            modelBuilder.ApplyConfiguration(new ClientAddressConfiguration(tenantId));
        }
    }
}

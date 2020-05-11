using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.File;
using PanacheSoftware.Http;
using PanacheSoftware.Service.File.Persistance.EntityConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.File.Persistance.Context
{
    public class PanacheSoftwareServiceFileContext : DbContext
    {
        private readonly IUserProvider _userProvider;

        public PanacheSoftwareServiceFileContext(DbContextOptions<PanacheSoftwareServiceFileContext> options, IUserProvider userProvider) : base(options)
        {
            _userProvider = userProvider;
        }

        public virtual DbSet<FileHeader> FileHeaders { get; set; }
        public virtual DbSet<FileDetail> FileDetails { get; set; }
        public virtual DbSet<FileVersion> FileVersions { get; set; }
        public virtual DbSet<FileLink> FileLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var tenantId = _userProvider.GetTenantId();

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new FileHeaderConfiguration(tenantId));
            modelBuilder.ApplyConfiguration(new FileDetailConfiguration(tenantId));
            modelBuilder.ApplyConfiguration(new FileVersionConfiguration(tenantId));
            modelBuilder.ApplyConfiguration(new FileLinkConfiguration(tenantId));
        }
    }
}

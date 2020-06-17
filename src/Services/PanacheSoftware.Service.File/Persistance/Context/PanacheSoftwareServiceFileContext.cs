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

            modelBuilder.ApplyConfiguration(new FileHeaderConfiguration());
            modelBuilder.ApplyConfiguration(new FileDetailConfiguration());
            modelBuilder.ApplyConfiguration(new FileVersionConfiguration());
            modelBuilder.ApplyConfiguration(new FileLinkConfiguration());

            modelBuilder.Entity<FileHeader>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<FileDetail>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<FileVersion>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<FileLink>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
        }
    }
}

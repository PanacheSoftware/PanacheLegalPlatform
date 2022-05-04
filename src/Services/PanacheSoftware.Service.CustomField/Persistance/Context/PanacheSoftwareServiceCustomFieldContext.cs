using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.CustomField;
using PanacheSoftware.Http;
using PanacheSoftware.Service.CustomField.Persistance.EntityConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.CustomField.Persistance.Context
{
    public class PanacheSoftwareServiceCustomFieldContext : DbContext
    {
        private readonly IUserProvider _userProvider;

        public PanacheSoftwareServiceCustomFieldContext(DbContextOptions<PanacheSoftwareServiceCustomFieldContext> options, IUserProvider userProvider) : base(options)
        {
            _userProvider = userProvider;
        }

        public virtual DbSet<CustomFieldHeader> CustomFieldHeaders { get; set; }
        public virtual DbSet<CustomFieldGroupHeader> CustomFieldGroupHeaders { get; set; }
        public virtual DbSet<CustomFieldGroupDetail> CustomFieldGroupDetails { get; set; }
        public virtual DbSet<CustomFieldTag> CustomFieldTags { get; set; }
        public virtual DbSet<CustomFieldValue> CustomFieldValues { get; set; }
        public virtual DbSet<CustomFieldValueHistory> CustomFieldValueHistorys {  get; set;  }
        public virtual DbSet<CustomFieldGroupLink> CustomFieldGroupLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CustomFieldHeaderConfiguration());
            modelBuilder.ApplyConfiguration(new CustomFieldGroupHeaderConfiguration());
            modelBuilder.ApplyConfiguration(new CustomFieldGroupDetailConfiguration());
            modelBuilder.ApplyConfiguration(new CustomFieldTagConfiguration());
            modelBuilder.ApplyConfiguration(new CustomFieldValueConfiguration());
            modelBuilder.ApplyConfiguration(new CustomFieldValueHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new CustomFieldGroupLinkConfiguration());

            modelBuilder.Entity<CustomFieldHeader>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<CustomFieldGroupHeader>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<CustomFieldGroupDetail>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<CustomFieldTag>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<CustomFieldValue>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<CustomFieldValueHistory>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<CustomFieldGroupLink>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
        }
    }
}

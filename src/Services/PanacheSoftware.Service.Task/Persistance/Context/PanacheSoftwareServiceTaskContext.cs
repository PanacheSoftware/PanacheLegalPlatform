using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Task;
using PanacheSoftware.Core.Domain.Join;
using PanacheSoftware.Service.Task.Persistance.EntityConfiguration;
using PanacheSoftware.Http;
using System;
using PanacheSoftware.Core.Domain.Task.Template;

namespace PanacheSoftware.Service.Task.Persistance.Context
{
    public class PanacheSoftwareServiceTaskContext : DbContext
    {
        private readonly IUserProvider _userProvider;
        public PanacheSoftwareServiceTaskContext(DbContextOptions<PanacheSoftwareServiceTaskContext> options, IUserProvider userProvider) : base(options)
        {
            _userProvider = userProvider;
        }

        public virtual DbSet<TaskGroupHeader> TaskGroupHeaders { get; set; }
        public virtual DbSet<TaskGroupDetail> TaskGroupDetails { get; set; }
        public virtual DbSet<TaskHeader> TaskHeaders { get; set; }
        public virtual DbSet<TaskDetail> TaskDetails { get; set; }

        public virtual DbSet<TemplateDetail> TemplateDetails { get; set; }
        public virtual DbSet<TemplateGroupDetail> TemplateGroupDetails { get; set; }
        public virtual DbSet<TemplateGroupHeader> TemplateGroupHeaders { get; set; }
        public virtual DbSet<TemplateHeader> TemplateHeaders { get; set; }
        public virtual DbSet<TemplateItemDetail> TemplateItemDetails { get; set; }
        public virtual DbSet<TemplateItemHeader> TemplateItemHeaders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new TaskGroupHeaderConfiguration());
            modelBuilder.ApplyConfiguration(new TaskGroupDetailConfiguration());
            modelBuilder.ApplyConfiguration(new TaskHeaderConfiguration());
            modelBuilder.ApplyConfiguration(new TaskDetailConfiguration());

            modelBuilder.ApplyConfiguration(new TemplateHeaderConfiguration());
            modelBuilder.ApplyConfiguration(new TemplateDetailConfiguration());
            modelBuilder.ApplyConfiguration(new TemplateGroupHeaderConfiguration());
            modelBuilder.ApplyConfiguration(new TemplateGroupDetailConfiguration());
            modelBuilder.ApplyConfiguration(new TemplateItemHeaderConfiguration());
            modelBuilder.ApplyConfiguration(new TemplateItemDetailConfiguration());

            modelBuilder.Entity<TaskGroupHeader>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<TaskGroupDetail>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<TaskHeader>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<TaskDetail>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));

            modelBuilder.Entity<TemplateHeader>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<TemplateDetail>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<TemplateGroupHeader>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<TemplateGroupDetail>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<TemplateItemHeader>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<TemplateItemDetail>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
        }
    }
}

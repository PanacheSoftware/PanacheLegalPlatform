using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Task;
using PanacheSoftware.Core.Domain.Join;
using PanacheSoftware.Service.Task.Persistance.EntityConfiguration;
using PanacheSoftware.Http;
using System;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new TaskGroupHeaderConfiguration());
            modelBuilder.ApplyConfiguration(new TaskGroupDetailConfiguration());
            modelBuilder.ApplyConfiguration(new TaskHeaderConfiguration());
            modelBuilder.ApplyConfiguration(new TaskDetailConfiguration());

            modelBuilder.Entity<TaskGroupHeader>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<TaskGroupDetail>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<TaskHeader>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<TaskDetail>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
        }
    }
}

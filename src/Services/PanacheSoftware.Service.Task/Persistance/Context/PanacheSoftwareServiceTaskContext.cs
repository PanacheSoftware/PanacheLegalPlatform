using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Task;
using PanacheSoftware.Core.Domain.Join;
using PanacheSoftware.Service.Task.Persistance.EntityConfiguration;
using PanacheSoftware.Http;

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
            var tenantId = _userProvider.GetTenantId();

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new TaskGroupHeaderConfiguration(tenantId));
            modelBuilder.ApplyConfiguration(new TaskGroupDetailConfiguration(tenantId));
            modelBuilder.ApplyConfiguration(new TaskHeaderConfiguration(tenantId));
            modelBuilder.ApplyConfiguration(new TaskDetailConfiguration(tenantId));
        }
    }
}

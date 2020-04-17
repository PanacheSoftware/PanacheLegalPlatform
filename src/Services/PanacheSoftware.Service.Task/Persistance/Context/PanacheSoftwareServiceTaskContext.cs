using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Task;
using PanacheSoftware.Core.Domain.Join;
using PanacheSoftware.Service.Task.Persistance.EntityConfiguration;

namespace PanacheSoftware.Service.Task.Persistance.Context
{
    public class PanacheSoftwareServiceTaskContext : DbContext
    {
        public PanacheSoftwareServiceTaskContext(DbContextOptions<PanacheSoftwareServiceTaskContext> options) : base(options)
        {
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
        }
    }
}

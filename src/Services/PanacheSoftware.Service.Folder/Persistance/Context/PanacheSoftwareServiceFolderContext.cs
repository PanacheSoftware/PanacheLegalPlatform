using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Folder;
using PanacheSoftware.Core.Domain.Join;
using PanacheSoftware.Service.Folder.Persistance.EntityConfiguration;

namespace PanacheSoftware.Service.Folder.Persistance.Context
{
    public class PanacheSoftwareServiceFolderContext : DbContext
    {
        public PanacheSoftwareServiceFolderContext(DbContextOptions<PanacheSoftwareServiceFolderContext> options) : base(options)
        {
        }

        public virtual DbSet<FolderHeader> FolderHeaders { get; set; }
        public virtual DbSet<FolderDetail> FolderDetails { get; set; }
        public virtual DbSet<FolderNode> FolderNodes { get; set; }
        public virtual DbSet<FolderNodeDetail> FolderNodeDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new FolderHeaderConfiguration());
            modelBuilder.ApplyConfiguration(new FolderDetailConfiguration());
            modelBuilder.ApplyConfiguration(new FolderNodeConfiguration());
            modelBuilder.ApplyConfiguration(new FolderNodeDetailConfiguration());
        }
    }
}

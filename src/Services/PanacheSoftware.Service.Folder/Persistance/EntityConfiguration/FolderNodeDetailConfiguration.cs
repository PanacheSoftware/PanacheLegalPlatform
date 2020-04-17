using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Folder;

namespace PanacheSoftware.Service.Folder.Persistance.EntityConfiguration
{
    public class FolderNodeDetailConfiguration : IEntityTypeConfiguration<FolderNodeDetail>
    {
        public void Configure(EntityTypeBuilder<FolderNodeDetail> builder)
        {
            builder.ToTable("FolderNodeDetail");

            builder.Property(d => d.TenantId).IsRequired();
            builder.Property(d => d.Id).ValueGeneratedOnAdd();
            builder.Property(d => d.Status).IsRequired();

            builder.HasOne(d => d.FolderNode)
                .WithOne(h => h.FolderNodeDetail)
                .HasForeignKey<FolderNodeDetail>(d => d.FolderNodeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}


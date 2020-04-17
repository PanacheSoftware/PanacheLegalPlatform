using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Folder;

namespace PanacheSoftware.Service.Folder.Persistance.EntityConfiguration
{
    public class FolderNodeConfiguration : IEntityTypeConfiguration<FolderNode>
    {
        public void Configure(EntityTypeBuilder<FolderNode> builder)
        {
            builder.ToTable("FolderNode");

            builder.Property(d => d.TenantId).IsRequired();
            builder.Property(d => d.Id).ValueGeneratedOnAdd();
            builder.Property(d => d.Status).IsRequired();

            builder.HasOne(c => c.FolderHeader)
                .WithMany(h => h.ChildNodes)
                .HasForeignKey(c => c.FolderHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}


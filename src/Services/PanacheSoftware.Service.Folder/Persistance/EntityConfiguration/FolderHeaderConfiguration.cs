using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Folder;

namespace PanacheSoftware.Service.Folder.Persistance.EntityConfiguration
{
    public class FolderHeaderConfiguration : IEntityTypeConfiguration<FolderHeader>
    {
        public void Configure(EntityTypeBuilder<FolderHeader> builder)
        {
            builder.ToTable("FolderHeader");

            builder.Property(t => t.TenantId).IsRequired();
            builder.Property(t => t.Id).ValueGeneratedOnAdd();

            builder.Property(t => t.Status).IsRequired();
            builder.Property(t => t.ShortName).IsRequired();

            builder.HasOne(t => t.ParentFolder)
                .WithMany(t => t.ChildFolders)
                .HasForeignKey(t => t.ParentFolderId);
        }
    }
}

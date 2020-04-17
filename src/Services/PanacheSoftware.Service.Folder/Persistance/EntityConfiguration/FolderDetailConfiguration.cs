using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Folder;

namespace PanacheSoftware.Service.Folder.Persistance.EntityConfiguration
{
    public class FolderDetailConfiguration : IEntityTypeConfiguration<FolderDetail>
    {
        public void Configure(EntityTypeBuilder<FolderDetail> builder)
        {
            builder.ToTable("FolderDetail");

            builder.Property(d => d.TenantId).IsRequired();
            builder.Property(d => d.Id).ValueGeneratedOnAdd();
            builder.Property(d => d.Status).IsRequired();

            builder.HasOne(d => d.FolderHeader)
                .WithOne(h => h.FolderDetail)
                .HasForeignKey<FolderDetail>(d => d.FolderHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

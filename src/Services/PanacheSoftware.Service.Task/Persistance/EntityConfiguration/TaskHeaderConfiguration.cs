using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Task;

namespace PanacheSoftware.Service.Task.Persistance.EntityConfiguration
{
    public class TaskHeaderConfiguration : IEntityTypeConfiguration<TaskHeader>
    {
        public void Configure(EntityTypeBuilder<TaskHeader> builder)
        {
            builder.ToTable("TaskHeader");

            builder.Property(d => d.TenantId).IsRequired();
            builder.Property(d => d.Id).ValueGeneratedOnAdd();
            builder.Property(d => d.Status).IsRequired();

            builder.HasOne(c => c.TaskGroupHeader)
                .WithMany(h => h.ChildTasks)
                .HasForeignKey(c => c.TaskGroupHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

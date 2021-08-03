using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Task;
using System;

namespace PanacheSoftware.Service.Task.Persistance.EntityConfiguration
{
    public class TaskGroupHeaderConfiguration : IEntityTypeConfiguration<TaskGroupHeader>
    {
        public TaskGroupHeaderConfiguration()
        {
        }
        public void Configure(EntityTypeBuilder<TaskGroupHeader> builder)
        {
            builder.ToTable("TaskGroupHeader");

            builder.Property(t => t.TenantId).IsRequired();
            builder.Property(t => t.Id).ValueGeneratedOnAdd();

            builder.Property(t => t.Status).IsRequired();
            builder.Property(t => t.ShortName).IsRequired();

            builder.HasOne(t => t.ParentTaskGroup)
                .WithMany(t => t.ChildTaskGroups)
                .HasForeignKey(t => t.ParentTaskGroupId);
        }
    }
}

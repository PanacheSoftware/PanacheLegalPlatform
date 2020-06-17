using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Task;
using System;

namespace PanacheSoftware.Service.Task.Persistance.EntityConfiguration
{
    public class TaskDetailConfiguration : IEntityTypeConfiguration<TaskDetail>
    {
        public TaskDetailConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<TaskDetail> builder)
        {
            builder.ToTable("TaskDetail");

            builder.Property(d => d.TenantId).IsRequired();
            builder.Property(d => d.Id).ValueGeneratedOnAdd();
            builder.Property(d => d.Status).IsRequired();

            builder.HasOne(d => d.TaskHeader)
                .WithOne(h => h.TaskDetail)
                .HasForeignKey<TaskDetail>(d => d.TaskHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

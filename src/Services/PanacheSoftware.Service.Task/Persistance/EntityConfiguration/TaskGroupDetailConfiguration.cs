using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Task;
using System;

namespace PanacheSoftware.Service.Task.Persistance.EntityConfiguration
{
    public class TaskGroupDetailConfiguration : IEntityTypeConfiguration<TaskGroupDetail>
    {
        private string _tenantId;

        public TaskGroupDetailConfiguration(string tenantId)
        {
            _tenantId = tenantId;
        }
        public void Configure(EntityTypeBuilder<TaskGroupDetail> builder)
        {
            builder.ToTable("TaskGroupDetail");

            builder.Property(d => d.TenantId).IsRequired();
            builder.Property(d => d.Id).ValueGeneratedOnAdd();
            builder.Property(d => d.Status).IsRequired();

            builder.HasOne(d => d.TaskGroupHeader)
                .WithOne(h => h.TaskGroupDetail)
                .HasForeignKey<TaskGroupDetail>(d => d.TaskGroupHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(d => d.TenantId == Guid.Parse(_tenantId));
        }
    }
}

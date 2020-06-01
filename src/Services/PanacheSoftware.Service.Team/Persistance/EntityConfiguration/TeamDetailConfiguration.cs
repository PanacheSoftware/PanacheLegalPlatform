using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Team;
using System;

namespace PanacheSoftware.Service.Team.Persistance.EntityConfiguration
{
    public class TeamDetailConfiguration : IEntityTypeConfiguration<TeamDetail>
    {

        public TeamDetailConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<TeamDetail> builder)
        {
            builder.ToTable("TeamDetail");

            builder.Property(d => d.TenantId).IsRequired();
            builder.Property(d => d.Id).ValueGeneratedOnAdd();
            builder.Property(d => d.Status).IsRequired();

            builder.HasOne(d => d.TeamHeader)
                .WithOne(h => h.TeamDetail)
                .HasForeignKey<TeamDetail>(d => d.TeamHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

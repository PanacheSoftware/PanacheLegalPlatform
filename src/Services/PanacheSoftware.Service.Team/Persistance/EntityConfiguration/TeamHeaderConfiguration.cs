using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Team;
using System;

namespace PanacheSoftware.Service.Team.Persistance.EntityConfiguration
{
    public class TeamHeaderConfiguration : IEntityTypeConfiguration<TeamHeader>
    {
        private string _tenantId;

        public TeamHeaderConfiguration(string tenantId)
        {
            _tenantId = tenantId;
        }

        public void Configure(EntityTypeBuilder<TeamHeader> builder)
        {
            builder.ToTable("TeamHeader");

            builder.Property(t => t.TenantId).IsRequired();
            builder.Property(t => t.Id).ValueGeneratedOnAdd();

            builder.Property(t => t.Status).IsRequired();
            builder.Property(t => t.ShortName).IsRequired();

            builder.HasOne(t => t.ParentTeam)
                .WithMany(t => t.ChildTeams)
                .HasForeignKey(t => t.ParentTeamId);

            builder.HasQueryFilter(t => t.TenantId == Guid.Parse(_tenantId));
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.Join;
using System;

namespace PanacheSoftware.Service.Team.Persistance.EntityConfiguration
{
    public class UserTeamConfiguration : IEntityTypeConfiguration<UserTeam>
    {
        private string _tenantId;

        public UserTeamConfiguration(string tenantId)
        {
            _tenantId = tenantId;
        }

        public void Configure(EntityTypeBuilder<UserTeam> builder)
        {
            builder.ToTable("UserTeam");

            builder.Property(p => p.TenantId).IsRequired();
            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.Status).IsRequired();

            //builder.HasKey(k => new { k.UserId, k.TeamHeaderId });

            builder.HasQueryFilter(p => p.TenantId == Guid.Parse(_tenantId));
        }
    }
}
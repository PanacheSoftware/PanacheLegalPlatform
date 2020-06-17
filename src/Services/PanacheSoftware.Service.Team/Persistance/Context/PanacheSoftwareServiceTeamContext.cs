using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Join;
using PanacheSoftware.Core.Domain.Team;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Team.Persistance.EntityConfiguration;
using System;

namespace PanacheSoftware.Service.Team.Persistance.Context
{
    public class PanacheSoftwareServiceTeamContext : DbContext
    {
        private readonly IUserProvider _userProvider;

        public PanacheSoftwareServiceTeamContext(DbContextOptions<PanacheSoftwareServiceTeamContext> options, IUserProvider userProvider) : base(options)
        {
            _userProvider = userProvider;
        }

        public virtual DbSet<TeamHeader> TeamHeaders { get; set; }
        public virtual DbSet<TeamDetail> TeamDetails { get; set; }
        public virtual DbSet<UserTeam> UserTeams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new TeamHeaderConfiguration());
            modelBuilder.ApplyConfiguration(new TeamDetailConfiguration());
            modelBuilder.ApplyConfiguration(new UserTeamConfiguration());

            modelBuilder.Entity<TeamHeader>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<TeamDetail>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<UserTeam>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Join;
using PanacheSoftware.Core.Domain.Team;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Team.Persistance.EntityConfiguration;

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

            modelBuilder.ApplyConfiguration(new TeamHeaderConfiguration(_userProvider.GetTenantId()));
            modelBuilder.ApplyConfiguration(new TeamDetailConfiguration(_userProvider.GetTenantId()));
            modelBuilder.ApplyConfiguration(new UserTeamConfiguration(_userProvider.GetTenantId()));
        }
    }
}

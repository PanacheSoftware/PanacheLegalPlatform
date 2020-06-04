using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Core;
using PanacheSoftware.Database.Persistance;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Team.Core;
using PanacheSoftware.Service.Team.Core.Repositories;
using PanacheSoftware.Service.Team.Manager;
using PanacheSoftware.Service.Team.Persistance.Context;
using PanacheSoftware.Service.Team.Persistance.Repositories.Join;
using PanacheSoftware.Service.Team.Persistance.Repositories.Team;
using System;
using System.Linq;

namespace PanacheSoftware.Service.Team.Persistance
{
    public class UnitOfWork : PanacheSoftwareUnitOfWork, IUnitOfWork
    {
        public UnitOfWork(PanacheSoftwareServiceTeamContext context, IUserProvider userProvider) : base(context, userProvider)
        {
            TeamHeaders = new TeamHeaderRepository(context);
            TeamDetails = new TeamDetailRepository(context, TeamHeaders);
            UserTeams = new UserTeamRepository(context, TeamHeaders);
        }

        public ITeamHeaderRepository TeamHeaders { get; private set; }
        public ITeamDetailRepository TeamDetails { get; private set; }
        public IUserTeamRepository UserTeams { get; private set; }
    }
}

using PanacheSoftware.Database.Core;
using PanacheSoftware.Service.Team.Core.Repositories;
using System;

namespace PanacheSoftware.Service.Team.Core
{
    public interface IUnitOfWork : IPanacheSoftwareUnitOfWork
    {
        ITeamHeaderRepository TeamHeaders { get; }
        ITeamDetailRepository TeamDetails { get; }
        IUserTeamRepository UserTeams { get; }
    }
}

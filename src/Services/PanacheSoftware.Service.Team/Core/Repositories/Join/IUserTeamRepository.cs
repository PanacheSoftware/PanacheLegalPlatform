using PanacheSoftware.Core.Domain.Join;
using PanacheSoftware.Core.Domain.Team;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;

namespace PanacheSoftware.Service.Team.Core.Repositories
{
    public interface IUserTeamRepository : IPanacheSoftwareRepository<UserTeam>
    {
        UserTeam GetUserTeamWithRelations(Guid userTeamId, bool readOnly);
        //List<TeamHeader> GetTeamsForUser(string userName, bool readOnly);
        List<TeamHeader> GetTeamsForUser(Guid userDetailId, bool readOnly);
        //List<UserDetail> GetUsersForTeam(string teamShortName, bool readOnly);
        //List<UserDetail> GetUsersForTeam(Guid teamHeaderId, bool readOnly);
        //List<UserTeam> GetUserTeamsForUser(string userName, bool readOnly);
        List<UserTeam> GetUserTeamsForUser(Guid userId, bool readOnly);
        List<UserTeam> GetUserTeamsForTeam(string teamShortName, bool readOnly);
        List<UserTeam> GetUserTeamsForTeam(Guid teamHeaderId, bool readOnly);
    }
}

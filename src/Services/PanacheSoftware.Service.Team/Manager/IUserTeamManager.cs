using PanacheSoftware.Core.Domain.API.Join;
using PanacheSoftware.Core.Domain.API.Team;
using System;

namespace PanacheSoftware.Service.Team.Manager
{
    public interface IUserTeamManager
    {
        TeamList GetTeamsForUser(Guid userId);
        //UserList GetUsersForTeam(string teamShortName);
        //UserList GetUsersForTeam(Guid teamHeadId);
        //UserTeamJoinList GetUserTeamListForUser(string userName);
        UserTeamJoinList GetUserTeamListForUser(Guid userId);
        UserTeamJoinList GetUserTeamListForTeam(string teamShortName);
        UserTeamJoinList GetUserTeamListForTeam(Guid teamHeadId);
    }
}

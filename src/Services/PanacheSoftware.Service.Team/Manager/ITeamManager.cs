using PanacheSoftware.Core.Domain.API.Team;
using System;
using System.Collections.Generic;

namespace PanacheSoftware.Service.Team.Manager
{
    public interface ITeamManager
    {
        TeamList GetTeamList(Guid teamHeaderId = new Guid(), bool validParents = false);
        List<Guid> GetChildTeamIds(Guid teamHeaderId);
        TeamStruct GetTeamStructure(Guid teamHeaderId);
        TeamList GetTeamsForUser(Guid userDetailId);
        //UserList GetUsersForTeam(Guid teamHeaderId);
    }
}

using PanacheSoftware.Core.Domain.Team;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;

namespace PanacheSoftware.Service.Team.Core.Repositories
{
    public interface ITeamHeaderRepository : IPanacheSoftwareRepository<TeamHeader>
    {
        TeamHeader GetTeamHeader(string teamShortName, bool readOnly);
        TeamHeader GetTeamHeaderWithRelations(string teamShortName, bool readOnly);
        TeamHeader GetTeamHeaderWithRelations(Guid teamHeaderId, bool readOnly);
        Guid TeamNameToId(string teamShortName);
        List<TeamHeader> GetTeamTree(string teamShortName);
        List<TeamHeader> GetTeamTree(Guid teamHeaderId);
    }
}

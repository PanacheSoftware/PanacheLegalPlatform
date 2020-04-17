using PanacheSoftware.Core.Domain.Team;
using PanacheSoftware.Database.Core.Repositories;
using System;

namespace PanacheSoftware.Service.Team.Core.Repositories
{
    public interface ITeamDetailRepository : IPanacheSoftwareRepository<TeamDetail>
    {
        TeamDetail GetTeamDetail(string teamShortName, bool readOnly);
        TeamDetail GetTeamDetail(Guid teamHeaderId, bool readOnly);
        TeamDetail GetDetail(Guid teamDetailId, bool readOnly);
    }
}

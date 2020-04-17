using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Team;
using PanacheSoftware.Database.Repositories;
using PanacheSoftware.Service.Team.Core.Repositories;
using PanacheSoftware.Service.Team.Persistance.Context;
using System;
using System.Linq;

namespace PanacheSoftware.Service.Team.Persistance.Repositories.Team
{
    public class TeamDetailRepository : PanacheSoftwareRepository<TeamDetail>, ITeamDetailRepository
    {
        private readonly ITeamHeaderRepository _teamHeaderRepository;

        public TeamDetailRepository(PanacheSoftwareServiceTeamContext context, ITeamHeaderRepository teamHeaderRepository) : base(context)
        {
            _teamHeaderRepository = teamHeaderRepository;
        }

        public PanacheSoftwareServiceTeamContext PanacheSoftwareServiceTeamContext
        {
            get { return Context as PanacheSoftwareServiceTeamContext; }
        }

        public TeamDetail GetTeamDetail(string teamShortName, bool readOnly)
        {
            return GetTeamDetail(_teamHeaderRepository.TeamNameToId(teamShortName), readOnly);
        }

        public TeamDetail GetTeamDetail(Guid teamHeaderId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceTeamContext.TeamDetails.AsNoTracking().FirstOrDefault(t => t.TeamHeaderId == teamHeaderId);

            return PanacheSoftwareServiceTeamContext.TeamDetails.FirstOrDefault(t => t.TeamHeaderId == teamHeaderId);
        }

        public TeamDetail GetDetail(Guid teamDetailId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceTeamContext.TeamDetails.AsNoTracking().SingleOrDefault(a => a.Id == teamDetailId);

            return PanacheSoftwareServiceTeamContext.TeamDetails.SingleOrDefault(a => a.Id == teamDetailId);
        }
    }
}

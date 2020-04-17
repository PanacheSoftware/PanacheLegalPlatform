using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Team;
using PanacheSoftware.Database.Repositories;
using PanacheSoftware.Service.Team.Core.Repositories;
using PanacheSoftware.Service.Team.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PanacheSoftware.Service.Team.Persistance.Repositories.Team
{
    public class TeamHeaderRepository : PanacheSoftwareRepository<TeamHeader>, ITeamHeaderRepository
    {
        public TeamHeaderRepository(PanacheSoftwareServiceTeamContext context) : base(context)
        {

        }

        public PanacheSoftwareServiceTeamContext PanacheSoftwareServiceTeamContext
        {
            get { return Context as PanacheSoftwareServiceTeamContext; }
        }

        public TeamHeader GetTeamHeader(string teamShortName, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceTeamContext.TeamHeaders.AsNoTracking().SingleOrDefault(c => c.Id == TeamNameToId(teamShortName));

            return PanacheSoftwareServiceTeamContext.TeamHeaders.Find(TeamNameToId(teamShortName));
        }

        public TeamHeader GetTeamHeaderWithRelations(string teamShortName, bool readOnly)
        {
            return GetTeamHeaderWithRelations(TeamNameToId(teamShortName), readOnly);
        }

        public TeamHeader GetTeamHeaderWithRelations(Guid teamHeaderId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceTeamContext.TeamHeaders
                .Include(t => t.TeamDetail)
                .Include(t => t.ChildTeams)
                .Include(t => t.ParentTeam)
                .AsNoTracking()
                .SingleOrDefault(t => t.Id == teamHeaderId);

            return PanacheSoftwareServiceTeamContext.TeamHeaders
                .Include(t => t.TeamDetail)
                .Include(t => t.ChildTeams)
                .Include(t => t.ParentTeam)
                .SingleOrDefault(t => t.Id == teamHeaderId);
        }

        public List<TeamHeader> GetTeamTree(string teamShortName)
        {
            return GetTeamTree(TeamNameToId(teamShortName));
        }

        public List<TeamHeader> GetTeamTree(Guid teamHeaderId)
        {
            return PanacheSoftwareServiceTeamContext.TeamHeaders
                .Include(t => t.TeamDetail)
                .Include(t => t.ChildTeams)
                .AsEnumerable()
                .Where(t => t.Id == teamHeaderId).ToList();
        }

        /// <summary>
        /// Returns the TeamHeader ID corresponding to a ShortName
        /// </summary>
        /// <param name="teamShortName">TeamHeader ShortName</param>
        /// <returns>A valid TeamHeader ID or Guid.Empty if no TeamHeader found</returns>
        public Guid TeamNameToId(string teamShortName)
        {
            Guid foundGuid = Guid.Empty;

            TeamHeader foundTeamHeader =
                PanacheSoftwareServiceTeamContext.TeamHeaders.AsNoTracking().SingleOrDefault(t => t.ShortName == teamShortName);

            if (foundTeamHeader != null)
                foundGuid = foundTeamHeader.Id;

            return foundGuid;
        }
    }
}

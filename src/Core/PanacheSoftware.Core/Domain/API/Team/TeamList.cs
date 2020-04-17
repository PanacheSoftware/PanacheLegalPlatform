using System.Collections.Generic;

namespace PanacheSoftware.Core.Domain.API.Team
{
    public class TeamList
    {
        public TeamList()
        {
            TeamHeaders = new List<TeamHead>();
        }

        public List<TeamHead> TeamHeaders { get; set; }
    }
}

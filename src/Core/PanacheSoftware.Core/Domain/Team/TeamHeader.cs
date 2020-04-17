using PanacheSoftware.Core.Domain.Core;
using PanacheSoftware.Core.Domain.Join;
using System;
using System.Collections.Generic;

namespace PanacheSoftware.Core.Domain.Team
{
    public class TeamHeader : PanacheSoftwareEntity
    {
        public TeamHeader()
        {
            TeamDetail = new TeamDetail();
            ChildTeams = new HashSet<TeamHeader>();
            UserTeams = new HashSet<UserTeam>();
        }

        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public Guid? ParentTeamId { get; set; }
        public virtual TeamHeader ParentTeam { get; set; }

        public virtual TeamDetail TeamDetail { get; set; }
        public virtual ICollection<TeamHeader> ChildTeams { get; set; }

        public virtual ICollection<UserTeam> UserTeams { get; set; }
    }
}

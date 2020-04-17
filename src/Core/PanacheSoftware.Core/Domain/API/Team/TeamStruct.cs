using System;
using System.Collections.Generic;

namespace PanacheSoftware.Core.Domain.API.Team
{
    public class TeamStruct
    {
        public TeamStruct()
        {
            ChildTeams = new List<TeamStruct>();
        }

        public Guid Id { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public List<TeamStruct> ChildTeams { get; set; }
    }
}

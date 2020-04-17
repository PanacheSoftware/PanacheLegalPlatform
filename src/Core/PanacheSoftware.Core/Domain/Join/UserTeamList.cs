using System.Collections.Generic;

namespace PanacheSoftware.Core.Domain.Join
{
    public class UserTeamList
    {
        public UserTeamList()
        {
            UserTeams = new List<UserTeam>();
        }

        public List<UserTeam> UserTeams { get; set; }
    }
}

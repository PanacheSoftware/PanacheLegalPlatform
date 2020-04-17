using System.Collections.Generic;

namespace PanacheSoftware.Core.Domain.API.Join
{
    public class UserTeamJoinList
    {
        public UserTeamJoinList()
        {
            UserTeamJoins = new List<UserTeamJoin>();
        }

        public List<UserTeamJoin> UserTeamJoins { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.Team
{
    public class TeamChart
    {
        public TeamChart()
        {
            TeamNodes = new List<TeamNode>();
        }

        public IList<TeamNode> TeamNodes { get; set; }
    }
}

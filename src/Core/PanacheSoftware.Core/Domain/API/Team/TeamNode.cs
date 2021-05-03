using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.Team
{
    public class TeamNode
    {
        public TeamNode()
        {
            Id = Guid.Empty;
            TeamName = string.Empty;
            ParentId = Guid.Empty;
        }

        public Guid Id { get; set; }
        public string TeamName { get; set; }
        public Guid ParentId { get; set; }
    }
}

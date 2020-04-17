using PanacheSoftware.Core.Domain.Core;
using PanacheSoftware.Core.Domain.Team;
using System;

namespace PanacheSoftware.Core.Domain.Join
{
    public class UserTeam : PanacheSoftwareEntity
    {
        public Guid UserId { get; set; }

        public Guid TeamHeaderId { get; set; }
        public virtual TeamHeader TeamHeader { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}

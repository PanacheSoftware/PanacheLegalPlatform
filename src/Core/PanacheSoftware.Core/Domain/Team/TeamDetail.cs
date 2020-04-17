using PanacheSoftware.Core.Domain.Core;
using System;

namespace PanacheSoftware.Core.Domain.Team
{
    public class TeamDetail : PanacheSoftwareEntity
    {
        public Guid TeamHeaderId { get; set; }
        public virtual TeamHeader TeamHeader { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}

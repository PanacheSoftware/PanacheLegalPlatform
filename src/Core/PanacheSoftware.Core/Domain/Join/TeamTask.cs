using PanacheSoftware.Core.Domain.Core;
using PanacheSoftware.Core.Domain.Task;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.Join
{
    public class TeamTask : PanacheSoftwareEntity
    {
        public Guid TeamHeaderId { get; set; }
        public Guid TaskGroupHeaderId { get; set; }
        public virtual TaskGroupHeader TaskGroupHeader { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}

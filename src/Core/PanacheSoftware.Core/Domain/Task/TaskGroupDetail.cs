using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.Task
{
    public class TaskGroupDetail : PanacheSoftwareEntity
    {
        public Guid TaskGroupHeaderId { get; set; }
        public virtual TaskGroupHeader TaskGroupHeader { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}

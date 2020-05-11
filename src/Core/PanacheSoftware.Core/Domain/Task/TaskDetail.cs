using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.Task
{
    public class TaskDetail : PanacheSoftwareEntity
    {
        public Guid TaskHeaderId { get; set; }
        public virtual TaskHeader TaskHeader { get; set; }
    }
}

using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.Task.Template
{
    public class TemplateGroupDetail : PanacheSoftwareEntity
    {
        public Guid TemplateGroupHeaderId { get; set; }
        public virtual TemplateGroupHeader TemplateGroupHeader { get; set; }

        public int TotalDays { get; set; }
        public int DaysOffset { get; set; }
    }
}

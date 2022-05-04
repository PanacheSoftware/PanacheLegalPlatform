using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.Task.Template
{
    public class TemplateItemDetail : PanacheSoftwareEntity
    {
        public Guid TemplateItemHeaderId { get; set; }
        public virtual TemplateItemHeader TemplateItemHeader { get; set; }

        public int TotalDays { get; set; }
        public int DaysOffset { get; set; }
    }
}

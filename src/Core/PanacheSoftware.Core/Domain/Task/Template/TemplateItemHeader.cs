using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.Task.Template
{
    public class TemplateItemHeader : PanacheSoftwareEntity
    {
        public TemplateItemHeader()
        {
            TemplateItemDetail = new TemplateItemDetail();
        }

        public Guid TemplateGroupHeaderId { get; set; }
        public virtual TemplateGroupHeader TemplateGroupHeader { get; set; }

        public string Description { get; set; }
        public string Title { get; set; }

        public string TaskType { get; set; }

        public bool Completed { get; set; }

        public int SequenceNumber { get; set; }
        

        public virtual TemplateItemDetail TemplateItemDetail { get; set; }
    }
}

using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.Task.Template
{
    public class TemplateDetail : PanacheSoftwareEntity
    {
        public TemplateDetail()
        {
            
        }

        public int TotalDays { get; set; }
        public string Description { get; set; }

        public Guid TemplateHeaderId { get; set; }
        public virtual TemplateHeader TemplateHeader { get; set; }
    }
}

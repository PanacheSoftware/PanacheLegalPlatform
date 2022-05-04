using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.Task.Template
{
    public class TemplateHeader : PanacheSoftwareEntity
    {
        public TemplateHeader()
        {
            TemplateDetail = new TemplateDetail();
        }

        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<TemplateGroupHeader> TemplateGroupHeaders { get; set; }

        public virtual TemplateDetail TemplateDetail { get; set; }
    }
}

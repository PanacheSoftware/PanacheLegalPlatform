using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.Task.Template
{
    public class TemplateGroupHeader : PanacheSoftwareEntity
    {
        public TemplateGroupHeader()
        {
            TemplateGroupDetail = new TemplateGroupDetail();
            TemplateItemHeaders = new HashSet<TemplateItemHeader>();
        }

        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }
        public int SequenceNumber { get; set; }

        public Guid TemplateHeaderId { get; set; }
        public virtual TemplateHeader TemplateHeader { get; set; }

        public virtual TemplateGroupDetail TemplateGroupDetail { get; set; }
        public virtual ICollection<TemplateItemHeader> TemplateItemHeaders { get; set; }
    }
}

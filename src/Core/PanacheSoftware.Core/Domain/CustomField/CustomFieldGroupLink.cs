using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.CustomField
{
    public class CustomFieldGroupLink : PanacheSoftwareEntity
    {
        public Guid LinkId { get; set; }
        public string LinkType { get; set; }
        public Guid CustomFieldGroupHeaderId { get; set; }
        public virtual CustomFieldGroupHeader CustomFieldGroupHeader { get; set; }
    }
}

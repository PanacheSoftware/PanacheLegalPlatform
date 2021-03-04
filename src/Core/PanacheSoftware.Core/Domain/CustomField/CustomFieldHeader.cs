using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.CustomField
{
    public class CustomFieldHeader : PanacheSoftwareEntity
    {
        public Guid CustomFieldGroupHeaderId { get; set; }
        public virtual CustomFieldGroupHeader CustomFieldGroupHeader { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string CustomFieldType { get; set; }

        public bool GDPR { get; set; }

        public bool History { get; set; }
        public bool Mandatory { get; set; }
        public int SequenceNo { get; set; }
    }
}

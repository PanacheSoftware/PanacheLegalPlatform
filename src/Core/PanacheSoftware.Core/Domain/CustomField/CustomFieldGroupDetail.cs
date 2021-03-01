using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.CustomField
{
    public class CustomFieldGroupDetail : PanacheSoftwareEntity
    {
        public Guid CustomFieldGroupHeaderId { get; set; }
        public virtual CustomFieldGroupHeader CustomFieldGroupHeader { get; set; }

        public Guid CustomFieldHeaderId { get; set; }
        public int SequenceNo { get; set; }
    }
}

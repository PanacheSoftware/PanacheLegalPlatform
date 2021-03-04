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

        public string FieldGroupType { get; set; }
        public string FieldGroupTag { get; set; }
    }
}

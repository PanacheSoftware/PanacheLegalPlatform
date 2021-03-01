using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.CustomField
{
    public class CustomFieldDetail : PanacheSoftwareEntity
    {
        public Guid CustomFieldHeaderId { get; set; }
        public virtual CustomFieldHeader CustomFieldHeader { get; set; }
    }
}

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
        public CustomFieldHeader()
        {
            CustomFieldDetail = new CustomFieldDetail();
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public string CustomFieldType { get; set; }

        public bool GDPR { get; set; }

        public bool History { get; set; }

        public virtual CustomFieldDetail CustomFieldDetail { get; set; }
    }
}

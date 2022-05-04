using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.CustomField
{
    public class CustomFieldValue : PanacheSoftwareEntity
    {
        public CustomFieldValue()
        {
            CustomFieldValueHistorys = new HashSet<CustomFieldValueHistory>();
        }

        public Guid CustomFieldHeaderId { get; set; }
        public virtual CustomFieldHeader CustomFieldHeader { get; set; }

        public Guid LinkId { get; set; }
        public string LinkType { get; set; }

        public string FieldValue { get; set; }

        public virtual ICollection<CustomFieldValueHistory> CustomFieldValueHistorys { get; set; }
    }
}

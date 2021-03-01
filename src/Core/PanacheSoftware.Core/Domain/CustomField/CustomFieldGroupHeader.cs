using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.CustomField
{
    public class CustomFieldGroupHeader : PanacheSoftwareEntity
    {
        public CustomFieldGroupHeader()
        {
            CustomFieldGroupDetails = new HashSet<CustomFieldGroupDetail>();
        }

        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<CustomFieldGroupDetail> CustomFieldGroupDetails { get; set; }
    }
}

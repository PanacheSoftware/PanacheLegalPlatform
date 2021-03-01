using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.CustomField
{
    public class CustomFieldGroupHead
    {
        public CustomFieldGroupHead()
        {
            CustomFieldGroupDetails = new List<CustomFieldGroupDet>();
            ShortName = string.Empty;
            LongName = string.Empty;
            Description = string.Empty;
            Status = StatusTypes.Open;
        }

        public Guid Id { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }

        public string Status { get; set; }

        public List<CustomFieldGroupDet> CustomFieldGroupDetails { get; set; }
    }
}

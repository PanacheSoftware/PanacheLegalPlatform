using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.CustomField
{
    public class CustomFieldGroupLnk
    {
        public CustomFieldGroupLnk()
        {
            Status = StatusTypes.Open;
            LinkType = string.Empty;
        }

        public Guid Id { get; set; }
        public Guid LinkId { get; set; }
        public string LinkType { get; set; }
        public Guid CustomFieldGroupHeaderId { get; set; }
        public virtual CustomFieldGroupHead CustomFieldGroupHeader { get; set; }
        public string Status { get; set; }
    }
}

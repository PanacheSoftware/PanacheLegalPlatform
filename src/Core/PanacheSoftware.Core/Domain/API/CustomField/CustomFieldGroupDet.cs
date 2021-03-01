using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.CustomField
{
    public class CustomFieldGroupDet
    {
        public CustomFieldGroupDet()
        {
            SequenceNo = 0;
            Status = StatusTypes.Open;
        }

        public Guid Id { get; set; }
        public Guid CustomFieldGroupHeaderId { get; set; }

        public Guid CustomFieldHeaderId { get; set; }
        public int SequenceNo { get; set; }
        public string Status { get; set; }
    }
}

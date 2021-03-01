using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.CustomField
{
    public class CustomFieldValHistr
    {
        public CustomFieldValHistr()
        {
            LinkType = LinkTypes.Task;
            SequenceNo = 0;
            OriginalCreationDate = DateTime.Now;
            Status = StatusTypes.Open;
        }

        public Guid Id { get; set; }
        public Guid CustomFieldHeaderId { get; set; }

        public Guid LinkId { get; set; }
        public string LinkType { get; set; }

        public string FieldValue { get; set; }

        public int SequenceNo { get; set; }

        public DateTime OriginalCreationDate { get; set; }
        public string Status { get; set; }
    }
}

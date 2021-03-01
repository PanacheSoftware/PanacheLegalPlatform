using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.CustomField
{
    public class CustomFieldValueHistory : PanacheSoftwareEntity
    {
        public Guid CustomFieldValueId { get; set; }
        public virtual CustomFieldValue CustomFieldValue { get; set; }

        public Guid LinkId { get; set; }
        public string LinkType { get; set; }

        public string FieldValue { get; set; }

        public int SequenceNo { get; set; }

        public DateTime OriginalCreationDate { get; set; }
    }
}

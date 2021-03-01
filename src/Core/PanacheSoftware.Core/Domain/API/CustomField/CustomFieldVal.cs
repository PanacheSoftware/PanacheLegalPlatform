using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.CustomField
{
    public class CustomFieldVal
    {
        public CustomFieldVal()
        {
            CustomFieldValueHistorys = new List<CustomFieldValHistr>();
            LinkType = LinkTypes.Task;
            FieldValue = string.Empty;
            Status = StatusTypes.Open;
        }

        public Guid Id { get; set; }
        public Guid CustomFieldHeaderId { get; set; }

        public Guid LinkId { get; set; }
        public string LinkType { get; set; }

        public string FieldValue { get; set; }
        public string Status { get; set; }

        public List<CustomFieldValHistr> CustomFieldValueHistorys { get; set; }
    }
}

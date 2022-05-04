using PanacheSoftware.Core.Domain.API.CustomField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.UI
{
    public class CustomFieldGroupValuesModel
    {
        public CustomFieldGroupValuesModel()
        {
            CustomFieldValues = new List<CustomFieldVal>();
        }

        public Guid LinkId { get; set; }
        public string LinkType { get; set; }
        public CustomFieldGroupHead customFieldGroupHeader { get; set; }
        public List<CustomFieldVal> CustomFieldValues { get; set; }
    }
}

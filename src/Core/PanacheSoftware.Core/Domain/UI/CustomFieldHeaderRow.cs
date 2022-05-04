using PanacheSoftware.Core.Domain.API.CustomField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.UI
{
    public class CustomFieldHeaderRow
    {
        public CustomFieldHeaderRow()
        {
            CustomFieldHeaderRowFields = new List<CustomFieldHeaderRowField>();
        }

        public List<CustomFieldHeaderRowField> CustomFieldHeaderRowFields { get; set; }
    }

    
}

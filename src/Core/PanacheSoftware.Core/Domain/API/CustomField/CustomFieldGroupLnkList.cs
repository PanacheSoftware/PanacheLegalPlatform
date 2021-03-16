using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.CustomField
{
    public class CustomFieldGroupLnkList
    {
        public CustomFieldGroupLnkList()
        {
            CustomFieldGroupLinks = new List<CustomFieldGroupLnk>();
        }

        public List<CustomFieldGroupLnk> CustomFieldGroupLinks { get; set; }
    }
}

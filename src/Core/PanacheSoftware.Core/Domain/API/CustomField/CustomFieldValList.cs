using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.CustomField
{
    public class CustomFieldValList
    {
        public CustomFieldValList()
        {
            CustomFieldValues = new List<CustomFieldVal>();
        }

        public List<CustomFieldVal> CustomFieldValues { get; set; }
    }
}

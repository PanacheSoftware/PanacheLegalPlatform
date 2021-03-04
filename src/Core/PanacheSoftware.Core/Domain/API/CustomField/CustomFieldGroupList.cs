using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.CustomField
{
    public class CustomFieldGroupList
    {
        public CustomFieldGroupList()
        {
            CustomFieldGroupHeaders = new List<CustomFieldGroupHead>();
        }

        public List<CustomFieldGroupHead> CustomFieldGroupHeaders { get; set; }
    }


}

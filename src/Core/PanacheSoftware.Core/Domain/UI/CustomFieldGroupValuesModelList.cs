using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.UI
{
    public class CustomFieldGroupValuesModelList
    {
        public CustomFieldGroupValuesModelList()
        {
            CustomFieldGroupValuesModels = new List<CustomFieldGroupValuesModel>();
        }

        public List<CustomFieldGroupValuesModel> CustomFieldGroupValuesModels { get; set; }
    }
}

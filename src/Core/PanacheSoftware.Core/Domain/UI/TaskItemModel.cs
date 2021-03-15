using PanacheSoftware.Core.Domain.API.CustomField;
using PanacheSoftware.Core.Domain.API.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.UI
{
    public class TaskItemModel
    {
        public TaskItemModel()
        {
            customFieldGroupLinks = new List<CustomFieldGroupLnk>();
        }
        public TaskHead taskHead { get; set; }
        public List<CustomFieldGroupLnk> customFieldGroupLinks { get; set; }
    }
}

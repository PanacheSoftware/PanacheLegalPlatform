using PanacheSoftware.Core.Domain.API.CustomField;
using PanacheSoftware.Core.Domain.API.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.UI
{
    public class TaskGroupModel
    {
        public TaskGroupModel()
        {
            customFieldGroupLinks = new List<CustomFieldGroupLnk>();
        }
        public TaskGroupHead taskGroupHead { get; set; }
        public List<CustomFieldGroupLnk> customFieldGroupLinks { get; set; }
    }
}

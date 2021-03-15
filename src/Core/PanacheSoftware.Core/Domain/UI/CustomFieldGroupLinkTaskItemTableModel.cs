using Microsoft.AspNetCore.Mvc.Rendering;
using PanacheSoftware.Core.Domain.API.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.UI
{
    public class CustomFieldGroupLinkTaskItemTableModel : CustomFieldGroupLinkTableModel
    {
        public TaskItemModel taskItemModel { get; set; }
    }
}

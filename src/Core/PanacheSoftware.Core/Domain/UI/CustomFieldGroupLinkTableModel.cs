using Microsoft.AspNetCore.Mvc.Rendering;
using PanacheSoftware.Core.Domain.API.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.UI
{
    public class CustomFieldGroupLinkTableModel
    {
        public SelectList StatusList { get; set; }
        public SelectList CustomFieldGroupSelectList { get; set; }
        public string[] customFieldGroupRows { get; set; }
        public LangQueryList langQueryList { get; set; }
    }
}

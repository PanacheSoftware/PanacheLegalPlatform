using Microsoft.AspNetCore.Mvc.Rendering;
using PanacheSoftware.Core.Domain.API.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.UI
{
    public class CustomFieldTableModel
    {
        public SelectList StatusList { get; set; }
        public SelectList CustomFieldTypeList { get; set; }
        public CustomFieldGroupModel customFieldGroupModel { get; set; }
        public string[] fieldListRows { get; set; }
        public LangQueryList langQueryList { get; set; }
    }
}

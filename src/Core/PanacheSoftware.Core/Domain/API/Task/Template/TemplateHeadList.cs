using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.Task.Template
{
    public class TemplateHeadList
    {
        public TemplateHeadList()
        {
            TemplateHeaders = new List<TemplateHead>();
        }

        public List<TemplateHead> TemplateHeaders { get; set; }
    }
}

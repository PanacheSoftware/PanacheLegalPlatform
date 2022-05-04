using PanacheSoftware.Core.Domain.API.Task.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.UI
{
    public class TemplateHeadModel
    {
        public TemplateHeadModel()
        {
            TemplateHeader = new TemplateHead();
            TaskGroupHeadId = Guid.Empty;
        }

        public TemplateHead TemplateHeader { get; set; }
        public Guid TaskGroupHeadId { get; set; }
    }
}

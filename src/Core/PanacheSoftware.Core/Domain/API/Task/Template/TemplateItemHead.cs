using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.Task.Template
{
    public class TemplateItemHead
    {
        public TemplateItemHead()
        {
            Status = StatusTypes.Open;
            TemplateItemDetail = new TemplateItemDet();
        }

        public Guid Id { get; set; }
        public Guid TemplateGroupHeaderId { get; set; }

        public string Description { get; set; }
        public string Title { get; set; }

        public string TaskType { get; set; }

        public int SequenceNumber { get; set; }


        public TemplateItemDet TemplateItemDetail { get; set; }

        [Required]
        public string Status { get; set; }
    }
}

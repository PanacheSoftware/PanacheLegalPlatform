using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.Task.Template
{
    public class TemplateItemDet
    {
        public TemplateItemDet()
        {
            Status = StatusTypes.Open;
        }

        public Guid Id { get; set; }
        public Guid TemplateItemHeaderId { get; set; }

        public int TotalDays { get; set; }
        public int DaysOffset { get; set; }

        [Required]
        public string Status { get; set; }
    }
}

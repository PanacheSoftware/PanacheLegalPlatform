using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.Task.Template
{
    public class TemplateDet
    {
        public TemplateDet()
        {
            Status = StatusTypes.Open;
        }

        public Guid Id { get; set; }
        public Guid TemplateHeaderId { get; set; }
        [Required]
        public string Status { get; set; }

        public int TotalDays { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Description { get; set; }
    }
}

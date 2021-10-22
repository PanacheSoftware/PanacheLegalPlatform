using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.Task.Template
{
    public class TemplateHead
    {
        public TemplateHead()
        {
            TemplateDetail = new TemplateDet();
            Status = StatusTypes.Open;
            TemplateGroupHeaders = new List<TemplateGroupHead>();
        }

        public Guid Id { get; set; }
        [Required]
        [Display(Name = "Template ID")]
        [RegularExpression("^[A-Z0-9]*$", ErrorMessage = "Characters A-Z or 0-9 only")]
        [MaxLength(100, ErrorMessage = "Maximum Length 100 characters")]
        public string ShortName { get; set; }
        [Required]
        [Display(Name = "Template Name")]
        [MaxLength(255, ErrorMessage = "Maximum Length 255 characters")]
        public string LongName { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Description { get; set; }

        public TemplateDet TemplateDetail { get; set; }
        public List<TemplateGroupHead> TemplateGroupHeaders { get; set; }
        [Required]
        public string Status { get; set; }
    }
}

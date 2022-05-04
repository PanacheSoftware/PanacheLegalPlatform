using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.Task.Template
{
    public class TemplateGroupHead
    {
        public TemplateGroupHead()
        {
            TemplateGroupDetail = new TemplateGroupDet();
            TemplateItemHeaders = new List<TemplateItemHead>();
            Status = StatusTypes.Open;
        }

        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Template Group ID")]
        [RegularExpression("^[A-Z0-9]*$", ErrorMessage = "Characters A-Z or 0-9 only")]
        [MaxLength(100, ErrorMessage = "Maximum Length 100 characters")]
        public string ShortName { get; set; }
        [Required]
        [Display(Name = "Template Group Name")]
        [MaxLength(255, ErrorMessage = "Maximum Length 255 characters")]
        public string LongName { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Description { get; set; }
        
        public int SequenceNumber { get; set; }

        public Guid TemplateHeaderId { get; set; }

        public TemplateGroupDet TemplateGroupDetail { get; set; }
        public List<TemplateItemHead> TemplateItemHeaders { get; set; }
        [Required]
        public string Status { get; set; }
    }
}

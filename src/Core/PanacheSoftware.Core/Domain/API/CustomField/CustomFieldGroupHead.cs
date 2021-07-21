using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.CustomField
{
    public class CustomFieldGroupHead
    {
        public CustomFieldGroupHead()
        {
            CustomFieldGroupDetail = new CustomFieldGroupDet();
            CustomFieldHeaders = new List<CustomFieldHead>();
            ShortName = string.Empty;
            LongName = string.Empty;
            Description = string.Empty;
            Status = StatusTypes.Open;
        }

        public Guid Id { get; set; }
        [Required]
        [Display(Name = "Custom Field Group ID")]
        [RegularExpression("^[A-Z0-9]*$", ErrorMessage = "Characters A-Z or 0-9 only")]
        [MaxLength(100, ErrorMessage = "Maximum Length 100 characters")]
        public string ShortName { get; set; }
        [Required]
        [Display(Name = "Custom Field Group Name")]
        [MaxLength(255, ErrorMessage = "Maximum Length 255 characters")]
        public string LongName { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Description { get; set; }

        [Required]
        public string Status { get; set; }

        public CustomFieldGroupDet CustomFieldGroupDetail { get; set; }

        public List<CustomFieldHead> CustomFieldHeaders { get; set; }
    }
}

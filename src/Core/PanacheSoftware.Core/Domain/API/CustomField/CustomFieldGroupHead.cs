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
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }

        [Required]
        public string Status { get; set; }

        public CustomFieldGroupDet CustomFieldGroupDetail { get; set; }

        public List<CustomFieldHead> CustomFieldHeaders { get; set; }
    }
}

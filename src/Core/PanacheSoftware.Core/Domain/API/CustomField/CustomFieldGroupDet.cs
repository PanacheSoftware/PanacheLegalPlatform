using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.CustomField
{
    public class CustomFieldGroupDet
    {
        public CustomFieldGroupDet()
        {
            FieldGroupType = string.Empty;
            FieldGroupTag = string.Empty;
            Status = StatusTypes.Open;
        }

        public Guid Id { get; set; }
        public Guid CustomFieldGroupHeadId { get; set; }

        public string FieldGroupType { get; set; }
        public string FieldGroupTag { get; set; }
        [Required]
        public string Status { get; set; }
    }
}

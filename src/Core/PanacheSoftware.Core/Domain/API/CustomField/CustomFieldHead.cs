using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.CustomField
{
    public class CustomFieldHead
    {
        public CustomFieldHead()
        {
            Status = StatusTypes.Open;
            Name = string.Empty;
            Description = string.Empty;
            CustomFieldType = CustomFieldTypes.StringField;
            GDPR = false;
            History = false;
        }

        public Guid Id { get; set; }
        public Guid CustomFieldGroupHeaderId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public string CustomFieldType { get; set; }

        public bool GDPR { get; set; }

        public bool History { get; set; }
        public bool Mandatory { get; set; }
        public int SequenceNo { get; set; }


        [Required]
        public string Status { get; set; }
    }
}

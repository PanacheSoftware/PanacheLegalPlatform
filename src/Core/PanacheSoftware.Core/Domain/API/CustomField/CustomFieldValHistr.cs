using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.CustomField
{
    public class CustomFieldValHistr
    {
        public CustomFieldValHistr()
        {
            FieldValue = string.Empty;
            SequenceNo = 0;
            OriginalCreationDate = DateTime.Now;
            Status = StatusTypes.Open;
        }

        public Guid Id { get; set; }
        public Guid CustomFieldValueId { get; set; }


        public string FieldValue { get; set; }

        public int SequenceNo { get; set; }

        public DateTime OriginalCreationDate { get; set; }
        [Required]
        public string Status { get; set; }
    }
}

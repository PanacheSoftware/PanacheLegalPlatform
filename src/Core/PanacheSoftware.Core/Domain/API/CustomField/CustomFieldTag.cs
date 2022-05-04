using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.CustomField
{
    public class CustomFieldTg
    {
        public CustomFieldTg()
        {
            Status = StatusTypes.Open;
            Name = string.Empty;
            Description = string.Empty;
        }

        public Guid Id { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

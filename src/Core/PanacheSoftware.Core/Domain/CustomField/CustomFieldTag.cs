using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.CustomField
{
    public class CustomFieldTag : PanacheSoftwareEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

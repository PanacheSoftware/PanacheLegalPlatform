using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.Identity
{
    public class IdentityTenant
    {
        public Guid Id { get; set; }
        public string Domain { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string CreatedByEmail { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}

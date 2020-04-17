using System;

namespace PanacheSoftware.Core.Domain.Core
{
    public abstract class PanacheSoftwareEntity : MultiTenant
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid LastUpdateBy { get; set; }
    }
}

using System;


namespace PanacheSoftware.Core.Domain.Core
{
    public abstract class MultiTenant
    {
        public Guid TenantId { get; set; }
    }
}

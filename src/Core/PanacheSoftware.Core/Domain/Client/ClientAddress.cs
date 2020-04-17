using PanacheSoftware.Core.Domain.Generic;
using System;

namespace PanacheSoftware.Core.Domain.Client
{
    public class ClientAddress : Address
    {
        public Guid ClientContactId { get; set; }
        public virtual ClientContact ClientContact { get; set; }
    }
}

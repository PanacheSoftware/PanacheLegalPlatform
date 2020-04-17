using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;

namespace PanacheSoftware.Core.Domain.Client
{
    public class ClientContact : PanacheSoftwareEntity
    {
        public ClientContact()
        {
            ClientAddresses = new HashSet<ClientAddress>();
        }

        public Guid ClientHeaderId { get; set; }
        public virtual ClientHeader ClientHeader { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public virtual ICollection<ClientAddress> ClientAddresses { get; set; }
        public bool MainContact { get; set; }
    }
}

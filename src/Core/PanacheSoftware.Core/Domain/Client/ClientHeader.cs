using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;

namespace PanacheSoftware.Core.Domain.Client
{
    public class ClientHeader : PanacheSoftwareEntity
    {
        public ClientHeader()
        {
            ClientContacts = new HashSet<ClientContact>();
            ClientDetail = new ClientDetail();
        }

        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public virtual ClientDetail ClientDetail { get; set; }

        public virtual ICollection<ClientContact> ClientContacts { get; set; }
    }
}

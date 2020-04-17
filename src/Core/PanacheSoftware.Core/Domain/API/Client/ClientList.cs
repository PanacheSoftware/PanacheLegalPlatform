using System.Collections.Generic;

namespace PanacheSoftware.Core.Domain.API.Client
{
    public class ClientList
    {
        public ClientList()
        {
            ClientHeaders = new List<ClientHead>();
        }

        public List<ClientHead> ClientHeaders { get; set; }
    }
}

using PanacheSoftware.Core.Domain.Client;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;

namespace PanacheSoftware.Service.Client.Core.Repositories
{
    public interface IClientAddressRepository : IPanacheSoftwareRepository<ClientAddress>
    {
        IEnumerable<ClientAddress> GetClientAddresses(string clientShortName, bool readOnly);
        IEnumerable<ClientAddress> GetClientAddresses(Guid clientHeaderId, bool readOnly);
        IEnumerable<ClientAddress> GetClientContactAddresses(Guid clientContactId, bool readOnly);
        ClientAddress GetAddress(Guid clientAddressId, bool readOnly);
    }
}

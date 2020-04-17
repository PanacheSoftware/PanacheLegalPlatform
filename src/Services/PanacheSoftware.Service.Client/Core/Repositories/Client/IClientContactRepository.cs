using PanacheSoftware.Core.Domain.Client;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;

namespace PanacheSoftware.Service.Client.Core.Repositories
{
    public interface IClientContactRepository : IPanacheSoftwareRepository<ClientContact>
    {
        IEnumerable<ClientContact> GetClientContacts(string clientShortName, bool readOnly);
        IEnumerable<ClientContact> GetClientContacts(Guid clientHeaderId, bool readOnly);
        IEnumerable<ClientContact> GetClientContactsWithRelations(string clientShortName, bool readOnly);
        IEnumerable<ClientContact> GetClientContactsWithRelations(Guid clientHeaderId, bool readOnly);
    }
}

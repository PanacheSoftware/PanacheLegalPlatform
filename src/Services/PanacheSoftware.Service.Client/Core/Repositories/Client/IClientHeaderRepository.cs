using PanacheSoftware.Core.Domain.Client;
using PanacheSoftware.Core.Domain.Core;
using PanacheSoftware.Database.Core.Repositories;
using PanacheSoftware.Database.Domain;
using System;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Client.Core.Repositories
{
    public interface IClientHeaderRepository : IPanacheSoftwareRepository<ClientHeader>
    {
        ClientHeader GetClientHeader(string clientShortName, bool readOnly);
        ClientHeader GetClientHeaderWithRelations(string clientShortName, bool readOnly);
        ClientHeader GetClientHeaderWithRelations(Guid clientHeaderId, bool readOnly);
        Guid ClientNameToId(string shortName);
    }
}

using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Client;
using PanacheSoftware.Database.Repositories;
using PanacheSoftware.Service.Client.Core.Repositories;
using PanacheSoftware.Service.Client.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PanacheSoftware.Service.Client.Persistance.Repositories.Client
{
    public class ClientContactRepository : PanacheSoftwareRepository<ClientContact>, IClientContactRepository
    {
        private readonly IClientHeaderRepository _clientHeaderRepository;

        public ClientContactRepository(PanacheSoftwareServiceClientContext context, IClientHeaderRepository clientHeaderRepository) : base(context)
        {
            _clientHeaderRepository = clientHeaderRepository;
        }

        public PanacheSoftwareServiceClientContext PanacheSoftwareServiceClientContext
        {
            get { return Context as PanacheSoftwareServiceClientContext; }
        }

        public IEnumerable<ClientContact> GetClientContacts(string clientShortName, bool readOnly)
        {
            return GetClientContacts(_clientHeaderRepository.ClientNameToId(clientShortName), readOnly);
        }

        public IEnumerable<ClientContact> GetClientContacts(Guid clientHeaderId, bool readOnly)
        {
            return GetClientContactsIncludingRelations(clientHeaderId, false, readOnly);
        }

        public IEnumerable<ClientContact> GetClientContactsWithRelations(string clientShortName, bool readOnly)
        {
            return GetClientContactsWithRelations(_clientHeaderRepository.ClientNameToId(clientShortName), readOnly);
        }

        public IEnumerable<ClientContact> GetClientContactsWithRelations(Guid clientHeaderId, bool readOnly)
        {
            return GetClientContactsIncludingRelations(clientHeaderId, true, readOnly);
        }

        private IEnumerable<ClientContact> GetClientContactsIncludingRelations(Guid clientHeaderId, bool relations, bool readOnly)
        {
            if (relations)
            {
                if(readOnly)
                    return PanacheSoftwareServiceClientContext.ClientContacts.AsNoTracking().Include(c => c.ClientAddresses).Where(c => c.ClientHeaderId == clientHeaderId);

                return PanacheSoftwareServiceClientContext.ClientContacts.Include(c => c.ClientAddresses).Where(c => c.ClientHeaderId == clientHeaderId);
            }

            if(readOnly)
                return PanacheSoftwareServiceClientContext.ClientContacts.AsNoTracking().Where(c => c.ClientHeaderId == clientHeaderId);

            return PanacheSoftwareServiceClientContext.ClientContacts.Where(c => c.ClientHeaderId == clientHeaderId);
        }
    }
}

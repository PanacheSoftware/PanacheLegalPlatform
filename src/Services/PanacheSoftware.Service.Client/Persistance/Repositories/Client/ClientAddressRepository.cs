using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Client;
using PanacheSoftware.Service.Client.Persistance.Context;
using PanacheSoftware.Service.Client.Core.Repositories;
using PanacheSoftware.Database.Repositories;

namespace PanacheSoftware.Service.Client.Persistance.Repositories.Client
{
    public class ClientAddressRepository : PanacheSoftwareRepository<ClientAddress>, IClientAddressRepository
    {
        private readonly IClientContactRepository _clientContactRepository;

        public ClientAddressRepository(PanacheSoftwareServiceClientContext context, IClientContactRepository clientContactRepository) : base(context)
        {
            _clientContactRepository = clientContactRepository;
        }

        public PanacheSoftwareServiceClientContext PanacheSoftwareServiceClientContext
        {
            get { return Context as PanacheSoftwareServiceClientContext; }
        }

        public ClientAddress GetAddress(Guid clientAddressId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceClientContext.ClientAddresses.AsNoTracking().SingleOrDefault(a => a.Id == clientAddressId);

            return PanacheSoftwareServiceClientContext.ClientAddresses.SingleOrDefault(a => a.Id == clientAddressId);
        }

        public IEnumerable<ClientAddress> GetClientAddresses(string clientShortName, bool readOnly)
        {
            IEnumerable<ClientContact> clientContacts = _clientContactRepository.GetClientContactsWithRelations(clientShortName, readOnly);

            return CreateClientAddressEnumerable(clientContacts);
        }

        public IEnumerable<ClientAddress> GetClientAddresses(Guid clientHeaderId, bool readOnly)
        {
            IEnumerable<ClientContact> clientContacts = _clientContactRepository.GetClientContactsWithRelations(clientHeaderId, readOnly);

            return CreateClientAddressEnumerable(clientContacts);
        }

        public IEnumerable<ClientAddress> GetClientContactAddresses(Guid clientContactId, bool readOnly)
        {
            if(readOnly)
                return PanacheSoftwareServiceClientContext.ClientContacts.AsNoTracking().SingleOrDefault(c => c.Id == clientContactId).ClientAddresses;

            return PanacheSoftwareServiceClientContext.ClientContacts.SingleOrDefault(c => c.Id == clientContactId).ClientAddresses;
        }

        private IEnumerable<ClientAddress> CreateClientAddressEnumerable(IEnumerable<ClientContact> clientContactWithAddresses)
        {
            List<ClientAddress> clientAddresses = new List<ClientAddress>();

            foreach (var currentContact in clientContactWithAddresses)
            {
                clientAddresses.Concat(currentContact.ClientAddresses);
            }

            return clientAddresses;
        }
    }
}

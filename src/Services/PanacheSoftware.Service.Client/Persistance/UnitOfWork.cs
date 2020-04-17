using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Core;
using PanacheSoftware.Database.Persistance;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Client.Core;
using PanacheSoftware.Service.Client.Core.Repositories;
using PanacheSoftware.Service.Client.Persistance.Context;
using PanacheSoftware.Service.Client.Persistance.Repositories.Client;
using System;
using System.Linq;

namespace PanacheSoftware.Service.Client.Persistance
{
    public class UnitOfWork : PanacheSoftwareUnitOfWork, IUnitOfWork
    {
        public UnitOfWork(PanacheSoftwareServiceClientContext context, IUserProvider userProvider) : base(context, userProvider)
        {
            ClientHeaders = new ClientHeaderRepository(context);
            ClientContacts = new ClientContactRepository(context, ClientHeaders);
            ClientDetails = new ClientDetailRepository(context, ClientHeaders);
            ClientAddresses = new ClientAddressRepository(context, ClientContacts);
        }

        public IClientHeaderRepository ClientHeaders { get; private set; }

        public IClientContactRepository ClientContacts { get; private set; }

        public IClientDetailRepository ClientDetails { get; private set; }

        public IClientAddressRepository ClientAddresses { get; private set; }
    }
}

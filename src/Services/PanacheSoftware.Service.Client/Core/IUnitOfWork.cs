using System;
using PanacheSoftware.Database.Core;
using PanacheSoftware.Service.Client.Core.Repositories;

namespace PanacheSoftware.Service.Client.Core
{
    public interface IUnitOfWork : IPanacheSoftwareUnitOfWork
    {
        IClientHeaderRepository ClientHeaders { get; }
        IClientContactRepository ClientContacts { get; }
        IClientDetailRepository ClientDetails { get; }
        IClientAddressRepository ClientAddresses { get; }

        //void Complete(Guid guid);
    }
}

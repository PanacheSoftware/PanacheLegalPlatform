using PanacheSoftware.Core.Domain.Client;
using PanacheSoftware.Database.Core.Repositories;
using System;

namespace PanacheSoftware.Service.Client.Core.Repositories
{
    public interface IClientDetailRepository : IPanacheSoftwareRepository<ClientDetail>
    {
        ClientDetail GetClientDetail(string clientShortName, bool readOnly);
        ClientDetail GetClientDetail(Guid clientHeaderId, bool readOnly);
        ClientDetail GetDetail(Guid clientDetailId, bool readOnly);
    }
}

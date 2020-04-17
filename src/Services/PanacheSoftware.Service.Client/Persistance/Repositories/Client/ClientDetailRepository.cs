using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Client;
using PanacheSoftware.Service.Client.Persistance.Context;
using PanacheSoftware.Service.Client.Core.Repositories;
using PanacheSoftware.Database.Repositories;

namespace PanacheSoftware.Service.Client.Persistance.Repositories.Client
{
    public class ClientDetailRepository : PanacheSoftwareRepository<ClientDetail>, IClientDetailRepository
    {
        private readonly IClientHeaderRepository _clientHeaderRepository;

        public ClientDetailRepository(PanacheSoftwareServiceClientContext context, IClientHeaderRepository clientHeaderRepository) : base(context)
        {
            _clientHeaderRepository = clientHeaderRepository;
        }

        public PanacheSoftwareServiceClientContext PanacheLegalClientContext
        {
            get { return Context as PanacheSoftwareServiceClientContext; }
        }

        public ClientDetail GetClientDetail(string clientShortName, bool readOnly)
        {
            return GetClientDetail(_clientHeaderRepository.ClientNameToId(clientShortName), readOnly);
        }

        public ClientDetail GetClientDetail(Guid clientHeaderId, bool readOnly)
        {
            if(readOnly)
                return PanacheLegalClientContext.ClientDetails.AsNoTracking().FirstOrDefault(c => c.ClientHeaderId == clientHeaderId);

            return PanacheLegalClientContext.ClientDetails.FirstOrDefault(c => c.ClientHeaderId == clientHeaderId);
        }

        public ClientDetail GetDetail(Guid clientDetailId, bool readOnly)
        {
            if (readOnly)
                return PanacheLegalClientContext.ClientDetails.AsNoTracking().SingleOrDefault(a => a.Id == clientDetailId);

            return PanacheLegalClientContext.ClientDetails.SingleOrDefault(a => a.Id == clientDetailId);
        }
    }
}

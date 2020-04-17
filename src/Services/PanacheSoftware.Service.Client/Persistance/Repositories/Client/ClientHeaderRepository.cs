using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Client;
using PanacheSoftware.Database.Repositories;
using PanacheSoftware.Service.Client.Core.Repositories;
using PanacheSoftware.Service.Client.Persistance.Context;
using System;
using System.Linq;

namespace PanacheSoftware.Service.Client.Persistance.Repositories.Client
{
    public class ClientHeaderRepository : PanacheSoftwareRepository<ClientHeader>, IClientHeaderRepository
    {
        public ClientHeaderRepository(PanacheSoftwareServiceClientContext context) : base(context)
        {
        }

        public PanacheSoftwareServiceClientContext PanacheLegalClientContext
        {
            get { return Context as PanacheSoftwareServiceClientContext; }
        }

        public ClientHeader GetClientHeader(string clientShortName, bool readOnly)
        {
            if(readOnly)
                return PanacheLegalClientContext.ClientHeaders.AsNoTracking().SingleOrDefault(c => c.Id == ClientNameToId(clientShortName));

            return PanacheLegalClientContext.ClientHeaders.Find(ClientNameToId(clientShortName));
        }

        public ClientHeader GetClientHeaderWithRelations(string clientShortName, bool readOnly)
        {
            return GetClientHeaderWithRelations(ClientNameToId(clientShortName), readOnly);
        }

        public ClientHeader GetClientHeaderWithRelations(Guid clientHeaderId, bool readOnly)
        {
            if(readOnly)
                return PanacheLegalClientContext.ClientHeaders
                .Include(c => c.ClientDetail)
                .Include(c => c.ClientContacts).ThenInclude(ca => ca.ClientAddresses)
                .AsNoTracking()
                .SingleOrDefault(c => c.Id == clientHeaderId);

            return PanacheLegalClientContext.ClientHeaders
                .Include(c => c.ClientDetail)
                .Include(c => c.ClientContacts).ThenInclude(ca => ca.ClientAddresses)
                .SingleOrDefault(c => c.Id == clientHeaderId);
        }

        #region Private Functions
        /// <summary>
        /// Returns the ClientHeader ID corresponding to a ShortName
        /// </summary>
        /// <param name="shortName">ClientHeader ShortName</param>
        /// <returns>A valid ClientHeader ID or Guid.Empty if no ClientHeader found</returns>
        public Guid ClientNameToId(string shortName)
        {
            Guid foundGuid = Guid.Empty;

            ClientHeader foundClientHeader =
                PanacheLegalClientContext.ClientHeaders.AsNoTracking().SingleOrDefault(c => c.ShortName == shortName);

            if (foundClientHeader != null)
                foundGuid = foundClientHeader.Id;

            return foundGuid;
        }

        #endregion
    }
}

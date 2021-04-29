using AutoMapper;
using PanacheSoftware.Core.Domain.API.Client;
using PanacheSoftware.Core.Domain.Client;
using PanacheSoftware.Service.Client.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Client.Manager
{
    public class ClientManager : IClientManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ClientManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public ClientSummary GetClientSummary(string id)
        {
            ClientHeader clientHeader = null;
            ClientSummary clientSummary = null;

            if (Guid.TryParse(id, out Guid parsedId))
            {
                clientHeader = _unitOfWork.ClientHeaders.GetClientHeaderWithRelations(parsedId, true);
            }
            else
            {
                clientHeader = _unitOfWork.ClientHeaders.GetClientHeaderWithRelations(id, true);
            }

            if(clientHeader != null)
            {
                clientSummary = new ClientSummary()
                {
                    Id = clientHeader.Id,
                    ShortName = clientHeader.ShortName,
                    LongName = clientHeader.LongName,
                };

                var mainContact = clientHeader.ClientContacts.Where(c => c.MainContact == true).SingleOrDefault();

                if(mainContact != null)
                {
                    clientSummary.ContactId = mainContact.Id;
                    clientSummary.MainContactName = $"{mainContact.FirstName} {mainContact.LastName}";
                    clientSummary.Phone = mainContact.Phone;
                    clientSummary.Email = mainContact.Email;
                }
            }

            return clientSummary;
        }

        public bool ClientShortNameExists(string shortName, Guid existingGuid = default)
        {
            ClientHeader clientHeader;

            if(existingGuid == default)
                clientHeader = _unitOfWork.ClientHeaders.SingleOrDefault(c => c.ShortName == shortName, true);

            clientHeader = _unitOfWork.ClientHeaders.SingleOrDefault(c => c.ShortName == shortName && c.Id != existingGuid, true);

            return clientHeader == default ? false : true;
        }
    }
}

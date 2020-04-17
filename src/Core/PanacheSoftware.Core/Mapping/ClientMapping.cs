using AutoMapper;
using PanacheSoftware.Core.Domain.API.Client;
using PanacheSoftware.Core.Domain.Client;

namespace PanacheSoftware.Core.Mapping
{
    public class ClientMapping : Profile
    {
        public ClientMapping()
        {
            CreateMap<ClientHeader, ClientHead>();
            CreateMap<ClientDetail, ClientDet>();
            CreateMap<ClientContact, ClientCon>();
            CreateMap<ClientAddress, ClientAddr>();
            CreateMap<ClientHead, ClientHeader>();
            CreateMap<ClientDet, ClientDetail>();
            CreateMap<ClientCon, ClientContact>();
            CreateMap<ClientAddr, ClientAddress>();
        }
    }
}

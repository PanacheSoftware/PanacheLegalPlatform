using AutoMapper;
using PanacheSoftware.Core.Domain.API.Client;
using PanacheSoftware.Core.Domain.API.Join;
using PanacheSoftware.Core.Domain.API.Team;
using PanacheSoftware.Core.Domain.Client;
using PanacheSoftware.Core.Domain.Join;
using PanacheSoftware.Core.Domain.Team;

namespace PanacheSoftware.Core.Mapping
{
    public class TeamMapping : Profile
    {
        public TeamMapping()
        {
            CreateMap<TeamHeader, TeamHead>();
            CreateMap<TeamDetail, TeamDet>();
            CreateMap<TeamHead, TeamHeader>();
            CreateMap<TeamDet, TeamDetail>();
            CreateMap<TeamHeader, TeamStruct>();
            CreateMap<UserTeam, UserTeamJoin>();
            CreateMap<UserTeamJoin, UserTeam>();
            CreateMap<UserTeamList, UserTeamJoinList>();
            CreateMap<UserTeamJoinList, UserTeamList>();
        }
    }
}


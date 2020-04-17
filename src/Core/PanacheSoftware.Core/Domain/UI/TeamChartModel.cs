using AutoMapper;
using PanacheSoftware.Core.Domain.API.Team;
using System.Collections.Generic;

namespace PanacheSoftware.Core.Domain.UI
{
    public class TeamChartModel
    {
        //var datasource = {
        //        'name': 'Lao Lao',
        //        'title': 'general manager',
        //        'className': 'team',
        //        'children': [
        //            { 'name': 'Bo Miao', 'title': 'department manager', 'className': 'team' },
        //            {
        //                'name': 'Su Miao', 'title': 'department manager', 'className': 'team',
        //                'children': [
        //                    { 'name': 'Tie Hua', 'title': 'senior engineer', 'className': 'team' },
        //                    {
        //                        'name': 'Hei Hei', 'title': 'senior engineer', 'className': 'team',
        //                        'children': [
        //                            { 'name': 'Dan Dan', 'title': 'engineer', 'className': 'team' }
        //                        ]
        //                    },
        //                    { 'name': 'Pang Pang', 'title': 'senior engineer', 'className': 'team' }
        //                ]
        //            },
        //            { 'name': 'Hong Miao', 'title': 'department manager', 'className': 'team' }
        //        ]
        //    };

        public TeamChartModel()
        {
            children = new List<TeamChartModel>();
            className = "team";
        }

        public string name { get; set; }
        public string title { get; set; }
        public string className { get; set; }
        public List<TeamChartModel> children { get; set; }

    }

    public class TeamChartMappingProfile : Profile
    {
        public TeamChartMappingProfile()
        {
            CreateMap<TeamStruct, TeamChartModel>()
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.ShortName))
                .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.LongName))
                .ForMember(dest => dest.children, opt => opt.MapFrom(src => src.ChildTeams));
        }
    }
}

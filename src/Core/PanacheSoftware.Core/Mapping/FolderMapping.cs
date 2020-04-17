using AutoMapper;
using PanacheSoftware.Core.Domain.API.Folder;
using PanacheSoftware.Core.Domain.API.Join;
using PanacheSoftware.Core.Domain.Folder;
using PanacheSoftware.Core.Domain.Join;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using System;
using System.Linq;

namespace PanacheSoftware.Core.Mapping
{
    public class FolderMapping : Profile
    {
        public FolderMapping()
        {
            CreateMap<FolderHeader, FolderHead>();
            CreateMap<FolderDetail, FolderDet>();
            CreateMap<FolderHead, FolderHeader>();
            CreateMap<FolderDet, FolderDetail>();
            //CreateMap<FolderHeader, FolderStruct>();
            CreateMap<FolderNode, FolderNod>();
            CreateMap<FolderNod, FolderNode>();
            //CreateMap<FolderNode, FolderNodeStruct>();
            CreateMap<FolderNode, FolderStruct>()
                .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.FolderHeaderId))
                .ForMember(dest => dest.Completed, opt => opt.MapFrom(src => (src.CompletedOnDate != DateTime.Parse("01/01/1900")) ? true : false));
            CreateMap<FolderStruct, FolderStructureModel>()
                .ForMember(dest => dest.CompletionDate, opt => opt.MapFrom(src => (src.CompletionDate != DateTime.Parse("01/01/1900")) ? src.CompletionDate.ToShortDateString() : string.Empty))
                .ForMember(dest => dest.CompletedOnDate, opt => opt.MapFrom(src => (src.CompletedOnDate != DateTime.Parse("01/01/1900")) ? src.CompletedOnDate.ToShortDateString() : string.Empty))
                .ForMember(dest => dest.Folder, opt => opt.MapFrom(src => (src.NodeType == NodeTypes.Folder) ? true : false))
                .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.ChildFolders.Concat(src.ChildNodes)));
            CreateMap<FolderHeader, FolderStruct>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.LongName))
                .ForMember(dest => dest.NodeType, opt => opt.MapFrom(src => NodeTypes.Folder))
                .ForMember(dest => dest.Completed, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CompletionDate, opt => opt.MapFrom(src => DateTime.Parse("01/01/1900")))
                .ForMember(dest => dest.CompletedOnDate, opt => opt.MapFrom(src => DateTime.Parse("01/01/1900")))
                .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentFolderId ?? Guid.Empty));
            CreateMap<FolderNodeDetail, FolderNodDet>();
            CreateMap<FolderNodDet, FolderNodeDetail>();
        }
    }
}

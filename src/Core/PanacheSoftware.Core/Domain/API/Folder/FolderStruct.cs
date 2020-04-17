using AutoMapper;
using PanacheSoftware.Core.Domain.Folder;
using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;

namespace PanacheSoftware.Core.Domain.API.Folder
{
    //public class FolderStruct
    //{
    //    public FolderStruct()
    //    {
    //        ChildFolders = new List<FolderStruct>();
    //        ChildNodes = new List<FolderNodeStruct>();
    //    }

    //    public Guid Id { get; set; }
    //    public string ShortName { get; set; }
    //    public string LongName { get; set; }
    //    public int SequenceNumber { get; set; }
    //    public List<FolderStruct> ChildFolders { get; set; }
    //    public List<FolderNodeStruct> ChildNodes { get; set; }
    //}

    //public class FolderNodeStruct
    //{
    //    public FolderNodeStruct()
    //    {

    //    }

    //    public Guid Id { get; set; }

    //    public string Title { get; set; }

    //    public DateTime CompletionDate { get; set; }
    //    public DateTime CompletedOnDate { get; set; }

    //    public string NodeType { get; set; }

    //    public bool Completed { get; set; }

    //    public int SequenceNumber { get; set; }
    //}

    public class FolderStruct
    {
        public FolderStruct()
        {
            ChildFolders = new List<FolderStruct>();
            ChildNodes = new List<FolderStruct>();
        }

        public Guid Id { get; set; }
        public Guid ParentId { get; set; }

        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Title { get; set; }
        public int SequenceNumber { get; set; }
        public string NodeType { get; set; }
        public bool Completed { get; set; }

        public DateTime CompletionDate { get; set; }
        public DateTime CompletedOnDate { get; set; }

        public List<FolderStruct> ChildFolders { get; set; }
        public List<FolderStruct> ChildNodes { get; set; }
    }

    //public class FolderStructureMappingProfile : Profile
    //{
    //    public FolderStructureMappingProfile()
    //    {
    //        //CreateMap<FolderHeader, FolderStruct>()
    //        //    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.LongName))
    //        //    .ForMember(dest => dest.NodeType, opt => opt.MapFrom(src => NodeTypes.Folder))
    //        //    .ForMember(dest => dest.Completed, opt => opt.MapFrom(src => false))
    //        //    .ForMember(dest => dest.CompletionDate, opt => opt.MapFrom(src => DateTime.Parse("01/01/1900")))
    //        //    .ForMember(dest => dest.CompletedOnDate, opt => opt.MapFrom(src => DateTime.Parse("01/01/1900")));
    //    }
    //}
}

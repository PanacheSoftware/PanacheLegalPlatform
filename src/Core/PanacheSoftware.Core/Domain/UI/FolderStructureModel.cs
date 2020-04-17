using AutoMapper;
using PanacheSoftware.Core.Domain.API.Folder;
using PanacheSoftware.Core.Domain.Folder;
using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PanacheSoftware.Core.Domain.UI
{
    //{
    //                    "title": "Books",
    //                    "expanded": true,
    //                    "folder": true,
    //                    "children": [
    //                        {
    //                        "title": "Art of War",
    //                        "type": "book",
    //                        "author": "Sun Tzu",
    //                        "year": -500,
    //                        "qty": 21,
    //                        "price": 5.95
    //                        },
    //                        {
    //                        "title": "The Hobbit",
    //                        "type": "book",
    //                        "author": "J.R.R. Tolkien",
    //                        "year": 1937,
    //                        "qty": 32,
    //                        "price": 8.97
    //                        },
    //                        {
    //                        "title": "The Little Prince",
    //                        "type": "book",
    //                        "author": "Antoine de Saint-Exupery",
    //                        "year": 1943,
    //                        "qty": 2946,
    //                        "price": 6.82
    //                        },
    //                        {
    //                        "title": "Don Quixote",
    //                        "type": "book",
    //                        "author": "Miguel de Cervantes",
    //                        "year": 1615,
    //                        "qty": 932,
    //                        "price": 15.99
    //                        }
    //                    ]
    //                },
    //                {
    //                    "title": "Music",
    //                    "folder": true,
    //                    "children": [
    //                        {
    //                        "title": "Nevermind",
    //                        "type": "music",
    //                        "author": "Nirvana",
    //                        "year": 1991,
    //                        "qty": 916,
    //                        "price": 15.95
    //                        },
    //                        {
    //                        "title": "Autobahn",
    //                        "type": "music",
    //                        "author": "Kraftwerk",
    //                        "year": 1974,
    //                        "qty": 2261,
    //                        "price": 23.98
    //                        },
    //                        {
    //                        "title": "Kind of Blue",
    //                        "type": "music",
    //                        "author": "Miles Davis",
    //                        "year": 1959,
    //                        "qty": 9735,
    //                        "price": 21.9
    //                        },
    //                        {
    //                        "title": "Back in Black",
    //                        "type": "music",
    //                        "author": "AC/DC",
    //                        "year": 1980,
    //                        "qty": 3895,
    //                        "price": 17.99
    //                        },
    //                        {
    //                        "title": "The Dark Side of the Moon",
    //                        "type": "music",
    //                        "author": "Pink Floyd",
    //                        "year": 1973,
    //                        "qty": 263,
    //                        "price": 17.99
    //                        },
    //                        {
    //                        "title": "Sgt. Pepper's Lonely Hearts Club Band",
    //                        "type": "music",
    //                        "author": "The Beatles",
    //                        "year": 1967,
    //                        "qty": 521,
    //                        "price": 13.98
    //                        }
    //                    ]
    //                }
    

    public class FolderStructureModel
    {
        public FolderStructureModel()
        {
            Children = new List<FolderStructureModel>();
        }

        public Guid Id { get; set; }
        public Guid ParentId { get; set; }

        public string Title { get; set; }
        public int SequenceNumber { get; set; }
        public string NodeType { get; set; }
        public bool Completed { get; set; }
        public bool Folder { get; set; }

        public string CompletionDate { get; set; }
        public string CompletedOnDate { get; set; }

        public List<FolderStructureModel> Children { get; set; }
    }

    //public class FolderStructureMappingProfile : Profile
    //{
    //    public FolderStructureMappingProfile()
    //    {
    //        //CreateMap<FolderStruct, FolderStructureModel>()
    //        //    .ForMember(dest => dest.Folder, opt => opt.MapFrom(src => (src.NodeType == NodeTypes.Folder) ? true : false))
    //        //    .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.ChildFolders.Concat(src.ChildNodes)));



    //            //.ForMember(dest => dest.completed, opt => opt.MapFrom(src => false))
    //            //.ForMember(dest => dest.duedate, opt => opt.MapFrom(src => DateTime.Parse("01/01/1900")))
    //            //.ForMember(dest => dest.completiondate, opt => opt.MapFrom(src => DateTime.Parse("01/01/1900")));
    //        //CreateMap<FolderNodeStruct, FolderStructureModel>()
    //        //    .ForMember(dest => dest.folder, opt => opt.MapFrom(src => false))
    //        //    .ForMember(dest => dest.nodeType, opt => opt.MapFrom(src => src.NodeType))
    //        //    .ForMember(dest => dest.duedate, opt => opt.MapFrom(src => src.CompletionDate))
    //        //    .ForMember(dest => dest.completiondate, opt => opt.MapFrom(src => src.CompletedOnDate));
    //    }
    //}
}

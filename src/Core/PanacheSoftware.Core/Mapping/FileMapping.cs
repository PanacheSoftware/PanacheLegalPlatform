using AutoMapper;
using PanacheSoftware.Core.Domain.API.File;
using PanacheSoftware.Core.Domain.File;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Mapping
{
    public class FileMapping : Profile
    {
        public FileMapping()
        {
            CreateMap<FileHeader, FileHead>();
            CreateMap<FileDetail, FileDet>();
            CreateMap<FileVersion, FileVer>();
            CreateMap<FileLink, FileLnk>();
            CreateMap<FileHead, FileHeader>();
            CreateMap<FileDet, FileDetail>();
            CreateMap<FileVer, FileVersion>();
            CreateMap<FileLnk, FileLink>();
        }
    }
}

using AutoMapper;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.Language;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Mapping
{
    public class LanguageMapping : Profile
    {
        public LanguageMapping()
        {
            CreateMap<LanguageHeader, LangHead>();
            CreateMap<LanguageItem, LangItem>();
            CreateMap<LanguageCode, LangCode>();
            CreateMap<LangHead, LanguageHeader>();
            CreateMap<LangItem, LanguageItem>();
            CreateMap<LangCode, LanguageCode>();
        }
    }
}

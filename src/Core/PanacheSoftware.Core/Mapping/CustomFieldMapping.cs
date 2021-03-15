using AutoMapper;
using PanacheSoftware.Core.Domain.API.CustomField;
using PanacheSoftware.Core.Domain.CustomField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Mapping
{
    public class CustomFieldMapping : Profile
    {
        public CustomFieldMapping()
        {
            CreateMap<CustomFieldHeader, CustomFieldHead>();
            CreateMap<CustomFieldHead, CustomFieldHeader>();
            CreateMap<CustomFieldGroupHeader, CustomFieldGroupHead>();
            CreateMap<CustomFieldGroupDetail, CustomFieldGroupDet>();
            CreateMap<CustomFieldGroupHead, CustomFieldGroupHeader>();
            CreateMap<CustomFieldGroupDet, CustomFieldGroupDetail>();
            CreateMap<CustomFieldGroupLink, CustomFieldGroupLnk>();
            CreateMap<CustomFieldGroupLnk, CustomFieldGroupLink>();
            CreateMap<CustomFieldValue, CustomFieldVal>();
            CreateMap<CustomFieldVal, CustomFieldValue>();
            CreateMap<CustomFieldValueHistory, CustomFieldValHistr>();
            CreateMap<CustomFieldValHistr, CustomFieldValueHistory>();
            CreateMap<CustomFieldTag, CustomFieldTg>();
            CreateMap<CustomFieldTg, CustomFieldTag>();
        }
    }
}

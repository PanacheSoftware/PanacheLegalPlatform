using AutoMapper;
using PanacheSoftware.Core.Domain.API.Task.Template;
using PanacheSoftware.Core.Domain.Task.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Mapping
{
    public class TemplateMapping : Profile
    {
        public TemplateMapping()
        {
            CreateMap<TemplateHeader, TemplateHead>();
            CreateMap<TemplateDetail, TemplateDet>();
            CreateMap<TemplateHead, TemplateHeader>();
            CreateMap<TemplateDet, TemplateDetail>();
            CreateMap<TemplateGroupHeader, TemplateGroupHead>();
            CreateMap<TemplateGroupDetail, TemplateGroupDet>();
            CreateMap<TemplateGroupHead, TemplateGroupHeader>();
            CreateMap<TemplateGroupDet, TemplateGroupDetail>();
            CreateMap<TemplateItemHeader, TemplateItemHead>();
            CreateMap<TemplateItemDetail, TemplateItemDet>();
            CreateMap<TemplateItemHead, TemplateItemHeader>();
            CreateMap<TemplateItemDet, TemplateItemDetail>();

        }
    }
}

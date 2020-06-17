using AutoMapper;
using PanacheSoftware.Core.Domain.API.Settings;
using PanacheSoftware.Core.Domain.Settings;

namespace PanacheSoftware.Core.Mapping
{
    public class SettingMapping : Profile
    {
        public SettingMapping()
        {
            CreateMap<SettingHeader, SettingHead>();
            CreateMap<SettingHead, SettingHeader>();
            CreateMap<UserSetting, UsrSetting>();
            CreateMap<UsrSetting, UserSetting>();
            CreateMap<TenantSetting, TenSetting>();
            CreateMap<TenSetting, TenantSetting>();
        }
    }
}

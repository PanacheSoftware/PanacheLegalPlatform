using AutoMapper;
using PanacheSoftware.Core.Domain.Identity;
using PanacheSoftware.Core.Domain.Identity.API;

namespace PanacheSoftware.Core.Mapping
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<ApplicationUser, UserModel>();
            CreateMap<UserModel, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
            CreateMap<UserViewModel, UserModel>();
            CreateMap<UserModel, UserViewModel>();
            CreateMap<UsersViewModel, UserListModel>();
            CreateMap<UserListModel, UsersViewModel>();
            CreateMap<CreateUserModel, UserModel>();
            CreateMap<UserModel, CreateUserModel>();
        }
    }
}

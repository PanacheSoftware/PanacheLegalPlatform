using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PanacheSoftware.Core.Domain.Identity.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Identity.Manager
{
    public interface IApplicationUserManager
    {
        Task<IdentityResult> CreateUserAsync(UserModel userModel, ModelStateDictionary modelState, string password, string passwordConfirm, Guid tenantId);
        Task<IdentityResult> UpdateUserAsync(UserModel userModel, ModelStateDictionary modelState);
        Task<IdentityResult> ChangeUserPassword(PasswordModel passwordModel, ModelStateDictionary modelState);
        Task<IdentityResult> CheckSeed();
    }
}

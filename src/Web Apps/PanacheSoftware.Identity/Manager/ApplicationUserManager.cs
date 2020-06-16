using AutoMapper;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PanacheSoftware.Core.Domain.Identity;
using PanacheSoftware.Core.Domain.Identity.API;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Identity.Data;
using System;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PanacheSoftware.Core.Domain.Configuration;

namespace PanacheSoftware.Identity.Manager
{
    public class ApplicationUserManager : IApplicationUserManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ApplicationUserManager(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
            _configuration = configuration;
        }

        public async Task<IdentityResult> ChangeUserPassword(PasswordModel passwordModel, ModelStateDictionary modelState)
        {
            var currentUser = await _userManager.FindByNameAsync(passwordModel.UserName);

            if (currentUser == null)
            {
                IdentityErrorDescriber identityError = new IdentityErrorDescriber();
                var getUserResult = IdentityResult.Failed(identityError.InvalidUserName(passwordModel.UserName));
                AddErrorsToModel(getUserResult, modelState);
                return getUserResult;
            }

            var passwordsMatch = ValidatePasswordsMatch(passwordModel.NewPassword, passwordModel.NewPasswordConfirm);

            if (!passwordsMatch.Succeeded)
            {
                modelState.AddModelError("NewPassword", passwordsMatch.Errors.First().Description);
                modelState.AddModelError("NewPasswordConfirm", passwordsMatch.Errors.First().Description);
                IdentityErrorDescriber identityError = new IdentityErrorDescriber();
                return IdentityResult.Failed(identityError.PasswordMismatch());
            }

            var result = await _userManager.ChangePasswordAsync(currentUser, passwordModel.CurrentPassword, passwordModel.NewPassword);

            if(!result.Succeeded)
            {
                AddErrorsToModel(result, modelState);
            }

            return result;
        }

        public async Task<IdentityResult> CheckSeed()
        {
            var admin = _userManager.FindByNameAsync("admin@panachesoftware.com").Result;

            if (admin == null)
            {
                var adminUser = new UserModel()
                {
                    Id = Guid.Empty,
                    FirstName = "Admin",
                    Surname = "User",
                    FullName = "Admin User",
                    //UserName = "admin",
                    Email = "admin@panachesoftware.com",
                    Status = StatusTypes.Open,
                    DateFrom = DateTime.Now,
                    DateTo = DateTime.Now.AddYears(99),
                    Description = "Admin User",
                    Base64ProfileImage = Base64Images.PanacheSoftwareDot
                };

                var identityTenant = new IdentityTenant()
                {
                    Domain = "panachesoftware.com",
                    CreatedByEmail = "admin@panachesoftware.com",
                    Description = "Default Panache Software tenant"
                };

                if(CreateTenant(identityTenant) != null)
                {
                    return await CreateUserAsync(adminUser, new ModelStateDictionary(), "Passw0rd123!", "Passw0rd123!", identityTenant.Id);
                }

                IdentityErrorDescriber identityError = new IdentityErrorDescriber();
                return IdentityResult.Failed(identityError.InvalidUserName("admin@panachesoftware.com"));
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> CreateUserAsync(UserModel userModel, ModelStateDictionary modelState, string password, string passwordConfirm, Guid tenantId)
        {
            var passwordsMatch = ValidatePasswordsMatch(password, passwordConfirm);
            IdentityErrorDescriber identityError = new IdentityErrorDescriber();

            if (!passwordsMatch.Succeeded)
            {
                modelState.AddModelError("password", passwordsMatch.Errors.First().Description);
                modelState.AddModelError("passwordConfirm", passwordsMatch.Errors.First().Description);
                return IdentityResult.Failed(identityError.PasswordMismatch());
            }

            var newUser = _mapper.Map<ApplicationUser>(userModel);

            var identityTenant = GetTenant(tenantId, string.Empty);

            if(identityTenant != null)
            {
                MailAddress foundEmail = new MailAddress(newUser.Email);
                if(foundEmail != null)
                {
                    if(identityTenant.Domain == foundEmail.Host)
                    {
                        var result = await _userManager.CreateAsync(newUser, password);

                        if (!result.Succeeded)
                        {
                            AddErrorsToModel(result, modelState);
                        }
                        else
                        {
                            var panacheSoftwareConfiguration = new PanacheSoftwareConfiguration();
                            _configuration.Bind("PanacheSoftware", panacheSoftwareConfiguration);

                            var UIClientURL = bool.Parse(panacheSoftwareConfiguration.CallMethod.UICallsSecure)
                                ? panacheSoftwareConfiguration.Url.UIClientURLSecure
                                : panacheSoftwareConfiguration.Url.UIClientURL;

                            result = _userManager.AddClaimsAsync(newUser, new Claim[]
                                {
                                    new Claim(JwtClaimTypes.Name, newUser.FullName),
                                    new Claim(JwtClaimTypes.GivenName, newUser.FirstName),
                                    new Claim(JwtClaimTypes.FamilyName, newUser.Surname),
                                    new Claim(JwtClaimTypes.Email, newUser.Email),
                                    new Claim(JwtClaimTypes.WebSite, $"{UIClientURL}/User/{newUser.Id}"),
                                    new Claim(JwtClaimTypes.Picture, $"{UIClientURL}/User/{newUser.Id}/ProfileImage"),
                                    //new Claim(JwtClaimTypes.WebSite, $"https://localhost:44380/User/{newUser.Id}"),
                                    //new Claim(JwtClaimTypes.Picture, $"https://localhost:44380/User/{newUser.Id}/ProfileImage"),
                                    new Claim(PanacheSoftwareClaims.TenantId, identityTenant.Id.ToString())
                                }).Result;

                            if (!result.Succeeded)
                            {
                                throw new Exception(result.Errors.First().Description);
                            }
                            else
                            {
                                userModel.Id = newUser.Id;
                            }
                        }

                        return result;
                    }
                }
            }

            return IdentityResult.Failed(identityError.InvalidUserName(newUser.Email));
        }

        public async Task<IdentityResult> UpdateUserAsync(UserModel userModel, ModelStateDictionary modelState)
        {
            var currentUser = await _userManager.FindByIdAsync(userModel.Id.ToString());

            if (currentUser == null)
            {
                currentUser = await _userManager.FindByNameAsync(userModel.Email);

                if(currentUser == null)
                {
                    IdentityErrorDescriber identityError = new IdentityErrorDescriber();
                    var result = IdentityResult.Failed(identityError.InvalidUserName(userModel.Email));
                    AddErrorsToModel(result, modelState);
                    return result;
                }
            }

            _mapper.Map(userModel, currentUser);

            currentUser.SecurityStamp = new Guid().ToString();
            var updateUserResult = await _userManager.UpdateAsync(currentUser);

            if(!updateUserResult.Succeeded)
            {
                AddErrorsToModel(updateUserResult, modelState);
                return updateUserResult;
            }

            return updateUserResult;
        }

        private void AddErrorsToModel(IdentityResult identityResult, ModelStateDictionary modelState)
        {
            foreach (var currentError in identityResult.Errors)
            {
                string currentKey = string.Empty;


                if (currentError.Code.Contains("Email"))
                {
                    currentKey = $"Email";
                }
                else if (currentError.Code.Contains("Password"))
                {
                    currentKey = $"password";
                }
                else
                {
                    currentKey = $"UserName";
                }

                if (modelState.ContainsKey(currentKey))
                {
                    if (modelState[currentKey].Errors.Count == 0)
                    {
                        modelState.AddModelError(currentKey, currentError.Description);
                    }
                }
            }
        }

        private IdentityResult ValidatePasswordsMatch(string password, string passwordConfirmation)
        {
            if(password != passwordConfirmation)
            {
                IdentityErrorDescriber identityError = new IdentityErrorDescriber();
                return IdentityResult.Failed(identityError.PasswordMismatch());
            }

            return IdentityResult.Success;
        }

        private IdentityTenant GetTenant(Guid tenantId = default(Guid), string domain = null)
        {
            if(!string.IsNullOrWhiteSpace(domain))
            {
                return _context.IdetityTenants.Where(t => t.Domain == domain).FirstOrDefault();
            }
            else if (tenantId != default(Guid))
            {
                return _context.IdetityTenants.Where(t => t.Id == tenantId).FirstOrDefault();
            }

            return null;
        }

        private IdentityTenant CreateTenant(IdentityTenant identityTenant)
        {
            //Ensure Tenant ID is empty
            identityTenant.Id = Guid.Empty;
            identityTenant.CreatedDate = DateTime.Now;
            identityTenant.DateFrom = identityTenant.CreatedDate;
            identityTenant.DateTo = identityTenant.DateFrom.AddYears(99);
            identityTenant.Status = StatusTypes.Open;

            var tenant = GetTenant(default(Guid), identityTenant.Domain);

            if (!string.IsNullOrWhiteSpace(identityTenant.Domain) && GetTenant(default(Guid), identityTenant.Domain) == null)
            {
                _context.IdetityTenants.Add(identityTenant);

                _context.SaveChanges();

                return identityTenant;
            }

            return null;
        }
    }
}

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PanacheSoftware.Core.Domain.Identity;
using PanacheSoftware.Core.Domain.Identity.API;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Identity.Manager;
using PanacheSoftware.Identity.PageHelpers;
using PanacheSoftware.UI.Core.Headers;

namespace PanacheSoftware.Identity.Pages.Account
{
    [SecurityHeaders]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public class LoginModel : PageModel
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IClientStore _clientStore;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEventService _events;
        private readonly IApplicationUserManager _applicationUserManager;

        [BindProperty]
        public LoginViewModel loginViewModel { get; set; }

        public LoginModel(
            IIdentityServerInteractionService interaction,
            IAuthenticationSchemeProvider schemeProvider,
            IClientStore clientStore,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEventService events,
            IApplicationUserManager applicationUserManager)
        {
            _interaction = interaction;
            _schemeProvider = schemeProvider;
            _clientStore = clientStore;
            _userManager = userManager;
            _signInManager = signInManager;
            _events = events;
            _applicationUserManager = applicationUserManager;
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl)
        {
            await _applicationUserManager.CheckSeed();
            // build a model so we know what to show on the login page
            loginViewModel = await BuildLoginViewModelAsync(returnUrl);

            if (loginViewModel.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToAction("Challenge", "External", new { provider = loginViewModel.ExternalLoginScheme, returnUrl });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string button)
        {
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(loginViewModel.ReturnUrl);

            // the user clicked the "cancel" button
            if (button != "login")
            {
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);



                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    if (context.IsNativeClient())
                    {
                        // if the client is PKCE then we assume it's native, so this change in how to
                        // return the response is for better UX for the end user.

                        //return View("Redirect", new RedirectViewModel { RedirectUrl = model.ReturnUrl });
                    }

                    return Redirect(loginViewModel.ReturnUrl);
                }
                else
                {
                    // since we don't have a valid context, then we just go back to the home page
                    return Redirect("~/");
                }
            }

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginViewModel.Username, loginViewModel.Password, loginViewModel.RememberLogin, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(loginViewModel.Username);
                    await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName));

                    if (context != null)
                    {
                        if (context.IsNativeClient())
                        {
                            // if the client is PKCE then we assume it's native, so this change in how to
                            // return the response is for better UX for the end user.

                            //return View("Redirect", new RedirectViewModel { RedirectUrl = model.ReturnUrl });
                        }

                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(loginViewModel.ReturnUrl);
                    }

                    // request for a local page
                    if (Url.IsLocalUrl(loginViewModel.ReturnUrl))
                    {
                        return Redirect(loginViewModel.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(loginViewModel.ReturnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new Exception("invalid return URL");
                    }
                }

                await _events.RaiseAsync(new UserLoginFailureEvent(loginViewModel.Username, "invalid credentials"));
                ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            var vm = await BuildLoginViewModelAsync(loginViewModel);
            return Page();
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null)
            {
                // this is meant to short circuit the UI and only trigger the one external IdP
                return new LoginViewModel
                {
                    EnableLocalLogin = false,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                    ExternalProviders = new ExternalProvider[] { new ExternalProvider { AuthenticationScheme = context.IdP } }
                };
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null ||
                            (x.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase))
                )
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.Client.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        //private async Task<IdentityResult> CheckForAndCreateSeedUsers()
        //{
           
        //    var admin = _userManager.FindByNameAsync("admin@panachesoftware.com").Result;

        //    if(admin == null)
        //    {
        //        var adminUser = new UserModel()
        //        {
        //            Id = Guid.Empty,
        //            FirstName = "Admin",
        //            Surname = "User",
        //            FullName = "Admin User",
        //            //UserName = "admin",
        //            Email = "admin@panachesoftware.com",
        //            Status = StatusTypes.Open,
        //            DateFrom = DateTime.Now,
        //            DateTo = DateTime.Now.AddYears(99),
        //            Description = "Admin User",
        //            Base64ProfileImage = Base64Images.PanacheSoftwareDot
        //        };

        //        return await _applicationUserManager.CreateUserAsync(adminUser, new ModelStateDictionary(), "Passw0rd123!", "Passw0rd123!");
        //    }

        //    return IdentityResult.Success;


        //    //if(admin == null)
        //    //{
        //    //    admin = new ApplicationUser
        //    //    {
        //    //        UserName = "admin",
        //    //    };
        //    //    var result = _userManager.CreateAsync(admin, "Passw0rd123!").Result;

        //    //    if(!result.Succeeded)
        //    //    {
        //    //        throw new Exception(result.Errors.First().Description);
        //    //    }

        //    //    result = _userManager.AddClaimsAsync(admin, new Claim[]
        //    //    {
        //    //        new Claim(JwtClaimTypes.Name, "Admin User"),
        //    //            new Claim(JwtClaimTypes.GivenName, "Admin"),
        //    //            new Claim(JwtClaimTypes.FamilyName, "User"),
        //    //            new Claim(JwtClaimTypes.Email, "admin.user@example.com"),
        //    //            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
        //    //            new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
        //    //            new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
        //    //    }).Result;

        //    //    if(!result.Succeeded)
        //    //    {
        //    //        throw new Exception(result.Errors.First().Description);
        //    //    }
        //    //}
        //}
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PanacheSoftware.Core.Domain.Identity;
using PanacheSoftware.Core.Domain.Identity.API;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Headers;

namespace PanacheSoftware.UI.Client.Pages.User
{
    [SecurityHeaders]
    [Authorize]
    [ValidateAntiForgeryToken]
    public class ProfileImageModel : PageModel
    {
        private readonly IAPIHelper _apiHelper;

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        public ProfileImageModel(
            IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<ActionResult> OnGetAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            string base64Image = Base64Images.PanacheSoftwareDot;

            if (!await _apiHelper.AuthCheck(accessToken, User.FindFirst("sub").Value))
            {
                return RedirectToPage("/Logout");
            }

            if (!string.IsNullOrWhiteSpace(Id))
            {
                if (Guid.TryParse(Id, out Guid foundId))
                {
                    if (foundId != Guid.Empty)
                    {
                        var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.IDENTITY, $"User/{foundId.ToString()}");

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            base64Image = response.ContentAsType<UserModel>().Base64ProfileImage;
                        }
                        //ApplicationUser applicationUser = await _userManager.FindByIdAsync(Id);

                        //if (applicationUser != null)
                        //{

                        //    base64Image = applicationUser.Base64ProfileImage;
                        //}
                    }
                }
            }

            var contentType = Regex.Match(base64Image, @"data:(.+?);").Groups[1].Value;
            var imageString = base64Image.Substring(base64Image.IndexOf($"data:{contentType};base64,") + $"data:{contentType};base64,".Length);

            byte[] bytes = Convert.FromBase64String(imageString);

            return File(bytes, contentType);
        }
    }
}
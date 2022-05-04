using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PanacheSoftware.Core.Domain.Identity.API;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;

namespace PanacheSoftware.UI.Client.Blazor.Pages.User
{
    //[SecurityHeaders]
    [Authorize]
    [ValidateAntiForgeryToken]
    public class ProfileImageModel : PageModel
    {
        private readonly IAPIHelper _apiHelper;
        private readonly TokenProvider tokenProvider;

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        public ProfileImageModel(TokenProvider tokenProvider, IAPIHelper apiHelper)
        {
            this.tokenProvider = tokenProvider;
            _apiHelper = apiHelper;
        }

        public async Task<ActionResult> OnGetAsync()
        {
            string base64Image = Base64Images.PanacheSoftwareDot;

            if (!string.IsNullOrWhiteSpace(Id))
            {
                if (Guid.TryParse(Id, out Guid foundId))
                {
                    if (foundId != Guid.Empty)
                    {
                        var response = await _apiHelper.MakeAPICallAsync(tokenProvider.AccessToken, HttpMethod.Get, APITypes.IDENTITY, $"User/{foundId}");

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            base64Image = response.ContentAsType<UserModel>().Base64ProfileImage;
                        }
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

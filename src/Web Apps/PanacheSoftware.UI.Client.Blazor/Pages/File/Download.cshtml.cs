using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PanacheSoftware.Core.Domain.API.File;
using PanacheSoftware.Core.Domain.Identity.API;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;

namespace PanacheSoftware.UI.Client.Blazor.Pages.File
{
    [Authorize]
    [ValidateAntiForgeryToken]
    public class DownloadModel : PageModel
    {
        private readonly IAPIHelper _apiHelper;
        private readonly TokenProvider tokenProvider;

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        public DownloadModel(TokenProvider tokenProvider, IAPIHelper apiHelper)
        {
            this.tokenProvider = tokenProvider;
            _apiHelper = apiHelper;
        }

        public async Task<ActionResult> OnGetAsync()
        {
            if (!string.IsNullOrWhiteSpace(Id))
            {
                if (Guid.TryParse(Id, out Guid parsedId))
                {
                    if (parsedId != Guid.Empty)
                    {
                        //var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.FILE, $"File/Version/{parsedId}");
                        var response = await _apiHelper.MakeAPICallAsync(tokenProvider.AccessToken, HttpMethod.Get, APITypes.FILE, $"File/Version/{parsedId}");

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var fileVersion = response.ContentAsType<FileVer>();

                            if (fileVersion != null)
                            {
                                return File(fileVersion.Content, MediaTypeNames.Application.Octet, fileVersion.TrustedName);
                            }
                        }
                    }
                }
            }

            return Page();
        }
    }
}

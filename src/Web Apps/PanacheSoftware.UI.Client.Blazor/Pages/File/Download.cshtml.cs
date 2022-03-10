using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PanacheSoftware.Core.Domain.API.Automation;
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
        private readonly TokenProvider _tokenProvider;

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        [BindProperty(SupportsGet = true)]
        public string VersionId { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool Automated { get; set; }

        public DownloadModel(TokenProvider tokenProvider, IAPIHelper apiHelper)
        {
            _tokenProvider = tokenProvider;
            _apiHelper = apiHelper;
        }

        public async Task<ActionResult> OnGetAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            if (!string.IsNullOrWhiteSpace(Id))
            {
                if (Guid.TryParse(Id, out Guid parsedId))
                {
                    if (parsedId != Guid.Empty)
                    {
                        if(!Automated)
                        {
                            //var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.FILE, $"File/Version/{parsedId}");
                            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.FILE, $"File/{parsedId}");

                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var fileHeader = response.ContentAsType<FileHead>();

                                if (Guid.TryParse(VersionId, out Guid versionId))
                                {
                                    var foundFileVersion = fileHeader.FileVersions.OrderByDescending(v => v.VersionNumber).FirstOrDefault();

                                    if (versionId != Guid.Empty)
                                    {
                                        foundFileVersion = fileHeader.FileVersions.FirstOrDefault(v => v.Id == versionId);
                                    }

                                    if (foundFileVersion != null)
                                    {
                                        return File(foundFileVersion.Content, fileHeader.FileDetail.FileType, foundFileVersion.TrustedName);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.AUTOMATION, $"Document/{parsedId}");

                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var autoDoc = response.ContentAsType<AutoDoc>();

                                return File(autoDoc.Content, autoDoc.FileType, autoDoc.TrustedName);
                            }
                        }
                    }
                }
            }

            return Page();
        }
    }
}

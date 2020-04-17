using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Headers;
using PanacheSoftware.UI.Core.Helpers;
using PanacheSoftware.UI.Core.PageModel;

namespace PanacheSoftware.UI.Client
{
    [SecurityHeaders]
    [Authorize]
    [ValidateAntiForgeryToken]
    public class LanguageCodeModel : PageModel, IPanacheSoftwarePageModel
    {
        private readonly IAPIHelper _apiHelper;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IModelHelper _modelHelper;
        [BindProperty]
        public LangCode langCode { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        public SelectList StatusList { get; }
        public LangQueryList langQueryList { get; set; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }

        public LanguageCodeModel(IAPIHelper apiHelper, IRazorPartialToStringRenderer renderer, IModelHelper modelHelper)
        {
            _apiHelper = apiHelper;
            _renderer = renderer;
            _modelHelper = modelHelper;
            StatusTypes statusTypes = new StatusTypes();
            StatusList = new SelectList(statusTypes.GetStatusTypesDictionary(), "Key", "Value");
        }

        public async Task<bool> PageConstructor(string saveState, string accessToken)
        {
            SaveState = saveState;

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10121, 10700, 10709, 10707, 10200, 10202, 10203, 10201, 10710, 10711, 10500, 10501 });

            return true;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            langCode = new LangCode();
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            if (!await _apiHelper.AuthCheck(accessToken, User.FindFirst("sub").Value))
            {
                return RedirectToPage("/Logout");
            }

            await PageConstructor(SaveStates.IGNORE, accessToken);

            if (!string.IsNullOrWhiteSpace(Id))
            {
                var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.FOUNDATION, $"Language/Code/{Id}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    langCode = response.ContentAsType<LangCode>();
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            await PageConstructor(SaveStates.IGNORE, accessToken);

            if (ModelState.IsValid)
            {
                if (langCode != null)
                {
                    await CreateOrUpdateLanguageCodeAsync(accessToken);
                }

                SaveState = SaveStates.SUCCESS;

                return Page();
            }

            SaveState = SaveStates.FAILED;

            return Page();
        }

        private async Task<bool> CreateOrUpdateLanguageCodeAsync(string apiAccessToken)
        {
            if (langCode != null)
            {
                if (langCode.Id != Guid.Empty)
                {
                    var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.FOUNDATION, $"Language/Code/{langCode.Id}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var foundLangCode = response.ContentAsType<LangCode>();

                        if (foundLangCode != null)
                        {
                            if (!await _modelHelper.ProcessPatch(foundLangCode, langCode, foundLangCode.Id, apiAccessToken, APITypes.FOUNDATION, "Language/Code"))
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(langCode), Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Post, APITypes.FOUNDATION, $"Language/Code", contentPost);

                        if (response.StatusCode != System.Net.HttpStatusCode.Created)
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        var responseString = $"Error calling API: {ex.Message}";
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
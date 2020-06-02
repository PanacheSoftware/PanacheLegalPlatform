using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Headers;
using PanacheSoftware.UI.Core.PageModel;

namespace PanacheSoftware.UI.Client.Pages.Setting
{
    [SecurityHeaders]
    [Authorize]
    [ValidateAntiForgeryToken]
    public class LanguageModel : PageModel, IPanacheSoftwarePageModel
    {
        private readonly IAPIHelper _apiHelper;
        public LangList languageList { get; set; }
        public LangCodeList langCodeList { get; set; }
        public LangQueryList langQueryList { get; set; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }
        public SaveMessageModel SaveMessageModel { get; set; }

        public LanguageModel(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }
        public async Task<bool> PageConstructor(string saveState, string accessToken)
        {
            SaveState = saveState;

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10121, 10119, 10712, 10702, 10703, 10200, 10201, 10202, 10203, 10204, 10713, 10707 });

            return true;
        }


        public async Task<IActionResult> OnGetAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            await PageConstructor(SaveStates.IGNORE, accessToken);

            if (!await _apiHelper.AuthCheck(accessToken, User.FindFirst("sub").Value))
            {
                return RedirectToPage("/Logout");
            }

            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.GATEWAY, $"Language");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                languageList = response.ContentAsType<LangList>();
            }
            else
            {
                languageList = new LangList();
            }

            response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.GATEWAY, $"Language/Code");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                langCodeList = response.ContentAsType<LangCodeList>();
            }
            else
            {
                langCodeList = new LangCodeList();
            }

            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken);

            return Page();
        }
    }
}
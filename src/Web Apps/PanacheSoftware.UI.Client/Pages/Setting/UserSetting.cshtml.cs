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
using PanacheSoftware.Core.Domain.API.Settings;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Headers;
using PanacheSoftware.UI.Core.Helpers;
using PanacheSoftware.UI.Core.PageModel;

namespace PanacheSoftware.UI.Client.Pages.Setting
{
    [SecurityHeaders]
    [Authorize]
    [ValidateAntiForgeryToken]
    public class UserSettingModel : PageModel, IPanacheSoftwarePageModel
    {
        private readonly IAPIHelper _apiHelper;
        private readonly IModelHelper _modelHelper;
        public LangQueryList langQueryList { get; set; }
        [BindProperty]
        public UsrSetting userLanguageSetting { get; set; }
        public SelectList LanguageCodeSelectList { get; set; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }
        public SaveMessageModel SaveMessageModel { get; set; }

        public UserSettingModel(IAPIHelper apiHelper, IModelHelper modelHelper)
        {
            _apiHelper = apiHelper;
            _modelHelper = modelHelper;
        }

        public async Task<bool> PageConstructor(string saveState, string accessToken)
        {
            SaveState = saveState;

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, userLanguageSetting.Value, new long[] { 10121, 10122, 10500, 10501, 10600, 10601 });

            if(!await CreateLanguageCodeSelectList(accessToken))
            {
                return false;
            }

            return true;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            if (!await _apiHelper.AuthCheck(accessToken, User.FindFirst("sub").Value))
            {
                return RedirectToPage("/Logout");
            }

            userLanguageSetting = await GetUserSetting("USER_LANGUAGE", accessToken);

            await PageConstructor(SaveStates.IGNORE, accessToken);

            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            await PageConstructor(SaveStates.IGNORE, accessToken);

            if (ModelState.IsValid)
            {
                if (userLanguageSetting != null)
                {
                    await CreateOrUpdateUserSettingAsync(accessToken);
                }

                SaveState = SaveStates.SUCCESS;

                SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken, SaveState);

                return Page();
            }

            SaveState = SaveStates.FAILED;

            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken, SaveState);

            return Page();
        }

        private async Task<bool> CreateOrUpdateUserSettingAsync(string apiAccessToken)
        {
            if (userLanguageSetting != null)
            {
                if (userLanguageSetting.Id != Guid.Empty)
                {
                    var foundUserLanguageSetting = await GetUserSetting("USER_LANGUAGE", apiAccessToken);

                    if (foundUserLanguageSetting != null)
                    {
                        if (!await _modelHelper.ProcessPatch(foundUserLanguageSetting, userLanguageSetting, foundUserLanguageSetting.Id, apiAccessToken, APITypes.GATEWAY, "Setting/UserSetting"))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(userLanguageSetting), Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Post, APITypes.GATEWAY, $"Setting/UserSetting", contentPost);

                        if (response.StatusCode != System.Net.HttpStatusCode.Created)
                        {
                            return false;
                        }

                        userLanguageSetting = response.ContentAsType<UsrSetting>();
                    }
                    catch (Exception ex)
                    {
                        ErrorString = $"Error calling API: {ex.Message}";
                        return false;
                    }
                }
            }

            return true;
        }

        private async Task<UsrSetting> GetUserSetting(string userSettingName, string accessToken)
        {
            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.GATEWAY, $"Setting/UserSetting/{userSettingName}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.ContentAsType<UsrSetting>();
            }

            return null;
        }

        private async Task<bool> CreateLanguageCodeSelectList(string accessToken)
        {
            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.GATEWAY, $"Language/Code");

            Dictionary<string, string> LanguageCodeDictionary = new Dictionary<string, string>();

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return false;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                LangCodeList langCodeList = response.ContentAsType<LangCodeList>();

                foreach (var langCode in langCodeList.LanguageCodes.OrderBy(l => l.LanguageCodeId))
                {
                    LanguageCodeDictionary.Add(langCode.LanguageCodeId, langCode.LanguageCodeId);
                }
            }

            LanguageCodeSelectList = new SelectList(LanguageCodeDictionary, "Key", "Value");

            return true;
        }
    }
}
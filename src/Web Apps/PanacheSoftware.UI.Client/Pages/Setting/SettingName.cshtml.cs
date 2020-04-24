﻿using System;
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
    public class SettingNameModel : PageModel, IPanacheSoftwarePageModel
    {
        private readonly IAPIHelper _apiHelper;
        private readonly IModelHelper _modelHelper;

        [BindProperty]
        public SettingHead settingHead { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        
        public LangQueryList langQueryList { get; set; }
        public SelectList SettingTypeList { get; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }

        public SettingNameModel(IAPIHelper apiHelper, IModelHelper modelHelper)
        {
            _apiHelper = apiHelper;
            _modelHelper = modelHelper;
            SettingTypes settingTypes = new SettingTypes();
            SettingTypeList = new SelectList(settingTypes.GetSettingTypesDictionary(), "Key", "Value");
        }

        public async Task<bool> PageConstructor(string saveState, string accessToken)
        {
            SaveState = saveState;

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10121, 10500, 10501, 10123, 10602, 10219, 10220, 10200, 10212, 10603, 10604 });

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

            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.FOUNDATION, $"Setting/{Id}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                settingHead = response.ContentAsType<SettingHead>();
            }
            else
            {
                settingHead = new SettingHead();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            await PageConstructor(SaveStates.IGNORE, accessToken);

            if (ModelState.IsValid)
            {
                if (settingHead != null)
                {
                    await CreateOrUpdateSystemSettingAsync(accessToken);
                }
                SaveState = SaveStates.SUCCESS;

                return Page();
            }

            SaveState = SaveStates.FAILED;

            return Page();
        }

        private async Task<bool> CreateOrUpdateSystemSettingAsync(string apiAccessToken)
        {
            if (settingHead != null)
            {
                if (settingHead.Id != Guid.Empty)
                {
                    var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.FOUNDATION, $"Setting/{settingHead.Id}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var foundSystemSetting = response.ContentAsType<SettingHead>();

                        if (foundSystemSetting != null)
                        {
                            if (!await _modelHelper.ProcessPatch(foundSystemSetting, settingHead, foundSystemSetting.Id, apiAccessToken, APITypes.FOUNDATION, "Setting"))
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(settingHead), Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Post, APITypes.FOUNDATION, $"Setting", contentPost);

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
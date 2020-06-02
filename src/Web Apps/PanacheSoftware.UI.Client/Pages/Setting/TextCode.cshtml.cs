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
    public class TextCodeModel : PageModel, IPanacheSoftwarePageModel
    {
        private readonly IAPIHelper _apiHelper;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IModelHelper _modelHelper;
        [BindProperty]
        public LangHead langHead { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        public SelectList StatusList { get; }
        public SelectList LanguageCodeSelectList { get; set; }
        public LanguageHeaderModel languageHeaderModel { get; set; }
        public string[] langItemRows { get; set; }
        public LangQueryList langQueryList { get; set; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }
        public SaveMessageModel SaveMessageModel { get; set; }

        public TextCodeModel(IAPIHelper apiHelper, IRazorPartialToStringRenderer renderer, IModelHelper modelHelper)
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

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10121, 10700, 10500, 10501, 10701, 10702, 10200, 10202, 10203, 10201, 10703, 10704, 10705, 10706, 10707, 10708 });

            await CreateLanguageCodeSelectList(accessToken);

            await GeneratePageConstructionModel();

            return true;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            langHead = new LangHead();
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            if (!await _apiHelper.AuthCheck(accessToken, User.FindFirst("sub").Value))
            {
                return RedirectToPage("/Logout");
            }

            if (!string.IsNullOrWhiteSpace(Id))
            {
                var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.GATEWAY, $"Language/{Id}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    langHead = response.ContentAsType<LangHead>();
                }
            }

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
                if (langHead != null)
                {
                    await CreateOrUpdateTextCodeAsync(accessToken);
                }

                SaveState = SaveStates.SUCCESS;

                SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken, SaveState);

                return Page();
            }

            SaveState = SaveStates.FAILED;

            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken, SaveState);

            return Page();
        }

        private async Task<bool> CreateOrUpdateTextCodeAsync(string apiAccessToken)
        {
            if (langHead != null)
            {
                if (langHead.Id != Guid.Empty)
                {
                    var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.GATEWAY, $"Language/{langHead.Id}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var foundLangHead = response.ContentAsType<LangHead>();

                        if (foundLangHead != null)
                        {
                            if (!await _modelHelper.ProcessPatch(foundLangHead, langHead, foundLangHead.Id, apiAccessToken, APITypes.GATEWAY, "Language"))
                            {
                                return false;
                            }
                        }

                        foreach (var langItem in langHead.LanguageItems)
                        {
                            if (!await CreateOrUpdateLanguageItemAsync(langItem, apiAccessToken))
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(langHead), Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Post, APITypes.GATEWAY, $"Language", contentPost);

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

        private async Task<bool> CreateOrUpdateLanguageItemAsync(LangItem langItem, string apiAccessToken)
        {
            if (langItem != null)
            {
                if (langItem.Id != Guid.Empty)
                {
                    var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.GATEWAY, $"Language/Item/{langItem.Id}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var foundItem = response.ContentAsType<LangItem>();

                        if (foundItem != null)
                        {
                            if (!await _modelHelper.ProcessPatch(foundItem, langItem, foundItem.Id, apiAccessToken, APITypes.GATEWAY, "Language/Item"))
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(langItem), Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Post, APITypes.GATEWAY, $"Language/Item", contentPost);

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

        private async Task<bool> GeneratePageConstructionModel()
        {
            LangItem dummyLangItem = new LangItem()
            {
                Id = Guid.Empty,
                LanguageHeaderId= Guid.Parse("11111111-1111-1111-1111-111111111111"),
                LanguageCodeId = "EN"
            };

            LanguageHeaderModel dummyLanguageHeaderModel = new LanguageHeaderModel()
            {
                langHead = new LangHead(),
                StatusList = StatusList,
                LanguageCodeSelectList = LanguageCodeSelectList
            };

            dummyLanguageHeaderModel.langHead.LanguageItems.Add(dummyLangItem);

            var langListHTML = await _renderer.RenderPartialToStringAsync("_LangItemTable", dummyLanguageHeaderModel);

            langListHTML = langListHTML.Replace("\n", "").Replace("\r", "").Replace("\t", "").ToString();

            var langItemListTableHTML = _renderer.GetStringBetween(langListHTML, "<!--LangItemTableRowsStart-->", "<!--LangItemTableRowsEnd-->");

            List<string> langItemListRows = langItemListTableHTML.TrimStart().TrimEnd().Replace("<td>", "").Split("</td>").ToList();

            for (int i = langItemListRows.Count - 1; i >= 0; i--)
            {
                var eol = ",";

                langItemListRows[i] = langItemListRows[i].TrimStart().TrimEnd();
                if (string.IsNullOrWhiteSpace(langItemListRows[i]))
                {
                    langItemListRows.RemoveAt(i);
                    continue;
                }

                if (i == langItemListRows.Count - 1)
                {
                    eol = "";
                }

                langItemListRows[i] = langItemListRows[i].Replace("langHead_LanguageItems_0__", "langHead_LanguageItems_' + rowCount + '__");
                langItemListRows[i] = langItemListRows[i].Replace("langHead.LanguageItems[0]", "langHead.LanguageItems[' + rowCount + ']");
                langItemListRows[i] = langItemListRows[i].Replace("11111111-1111-1111-1111-111111111111", "' + headerId + '");

                langItemListRows[i] = $"'{langItemListRows[i]}'{eol}";
            }

            langItemRows = langItemListRows.ToArray();

            languageHeaderModel = new LanguageHeaderModel()
            {
                langHead = this.langHead,
                StatusList = this.StatusList,
                LanguageCodeSelectList = this.LanguageCodeSelectList,
                langQueryList = this.langQueryList
            };

            return true;
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
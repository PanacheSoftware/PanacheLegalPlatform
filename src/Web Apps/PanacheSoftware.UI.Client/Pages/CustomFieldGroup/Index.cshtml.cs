using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PanacheSoftware.Core.Domain.API.CustomField;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Headers;
using PanacheSoftware.UI.Core.Helpers;
using PanacheSoftware.UI.Core.PageModel;

namespace PanacheSoftware.UI.Client.Pages.CustomFieldGroup
{
    [SecurityHeaders]
    [Authorize]
    [ValidateAntiForgeryToken]
    public class IndexModel : PageModel, IPanacheSoftwarePageModel
    {
        private readonly IAPIHelper _apiHelper;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IModelHelper _modelHelper;
        [BindProperty]
        public CustomFieldGroupModel customFieldGroupModel { get; set; }
        public CustomFieldTableModel customFieldTableModel { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        public SelectList StatusList { get; }
        public SelectList CustomFieldTypeList { get; }
        public LangQueryList langQueryList { get; set; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }
        public SaveMessageModel SaveMessageModel { get; set; }

        public IndexModel(IAPIHelper apiHelper, IRazorPartialToStringRenderer renderer, IModelHelper modelHelper)
        {
            _apiHelper = apiHelper;
            _renderer = renderer;
            _modelHelper = modelHelper;

            //SelectList creation
            StatusTypes statusTypes = new StatusTypes();
            CustomFieldTypes customFieldTypes = new CustomFieldTypes();
            StatusList = new SelectList(statusTypes.GetStatusTypesDictionary(), "Key", "Value");
            CustomFieldTypeList = new SelectList(customFieldTypes.GetCustomFieldTypesDictionary(), "Key", "Value");
        }

        public async Task<bool> PageConstructor(string saveState, string accessToken)
        {
            SaveState = saveState;

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10121, 10124, 10200, 10201, 10226, 10227, 10228, 11001, 11002, 11003, 11004, 11005, 11006, 11007, 11008, 11009 });

            await GeneratePageConstructionModel();

            customFieldTableModel.StatusList = StatusList;
            customFieldTableModel.customFieldGroupModel = customFieldGroupModel;
            customFieldTableModel.langQueryList = langQueryList;
            customFieldTableModel.CustomFieldTypeList = CustomFieldTypeList;

            return true;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            customFieldGroupModel = new CustomFieldGroupModel();
            customFieldGroupModel.customFieldGroupHead = new CustomFieldGroupHead();
            customFieldTableModel = new CustomFieldTableModel();

            await PageConstructor(SaveStates.IGNORE, accessToken);

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
                        var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.CUSTOMFIELDGROUP, $"CustomField/{foundId.ToString()}");

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            customFieldGroupModel.customFieldGroupHead = response.ContentAsType<CustomFieldGroupHead>();
                        }
                    }
                }
            }

            await PageConstructor(SaveStates.IGNORE, accessToken);

            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken);

            return Page();
        }

        private async Task<bool> GeneratePageConstructionModel()
        {
            CustomFieldTableModel dummyCustomFieldTableModel = new CustomFieldTableModel()
            {
                customFieldGroupModel = GenerateDummyCustomFieldGroupModel(),
                StatusList = StatusList,
                langQueryList = langQueryList,
                CustomFieldTypeList = CustomFieldTypeList
            };

            var customFieldListHTML = await _renderer.RenderPartialToStringAsync("_CustomFieldList", dummyCustomFieldTableModel);

            customFieldListHTML = customFieldListHTML.Replace("\n", "").Replace("\r", "").Replace("\t", "").ToString();

            var customFieldTableHTML = _renderer.GetStringBetween(customFieldListHTML, "<!--CustomFieldTableRowsStart-->", "<!--CustomFieldTableRowsEnd-->");

            List<string> customFieldListRows = customFieldTableHTML.TrimStart().TrimEnd().Replace("<td>", "").Split("</td>").ToList();

            for (int i = customFieldListRows.Count - 1; i >= 0; i--)
            {
                var eol = ",";

                customFieldListRows[i] = customFieldListRows[i].TrimStart().TrimEnd();
                if (string.IsNullOrWhiteSpace(customFieldListRows[i]))
                {
                    customFieldListRows.RemoveAt(i);
                    continue;
                }

                if (i == customFieldListRows.Count - 1)
                {
                    eol = "";
                }

                customFieldListRows[i] = customFieldListRows[i].Replace("customFieldGroupModel_CustomFieldHeaders_0__", "customFieldGroupModel_CustomFieldHeaders_' + rowCount + '__");
                customFieldListRows[i] = customFieldListRows[i].Replace("customFieldGroupModel.CustomFieldHeaders[0]", "customFieldGroupModel.CustomFieldHeaders[' + rowCount + ']");
                customFieldListRows[i] = customFieldListRows[i].Replace("11111111-1111-1111-1111-111111111111", "' + customFieldGroupHeadId + '");
                customFieldListRows[i] = customFieldListRows[i].Replace("disabled", string.Empty);

                customFieldListRows[i] = $"'{customFieldListRows[i]}'{eol}";
            }

            customFieldTableModel.fieldListRows = customFieldListRows.ToArray();

            return true;
        }

        private CustomFieldGroupModel GenerateDummyCustomFieldGroupModel()
        {
            var dummyCustomFieldGroupModel = new CustomFieldGroupModel()
            {
                customFieldGroupHead = new CustomFieldGroupHead()
            };

            dummyCustomFieldGroupModel.customFieldGroupHead.CustomFieldHeaders.Add(GenerateDummyCustomField());

            return dummyCustomFieldGroupModel;
        }

        private CustomFieldHead GenerateDummyCustomField()
        {
            var dummyCustomFieldHead = new CustomFieldHead()
            {
                Id = Guid.Empty,
                CustomFieldGroupHeadId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Description = string.Empty,
                CustomFieldType = CustomFieldTypes.StringField,
                Name = string.Empty,
                SequenceNo = 0,
                Mandatory = false,
                History = false,
                GDPR = false,
                Status = StatusTypes.Open
            };

            return dummyCustomFieldHead;
        }
    }
}

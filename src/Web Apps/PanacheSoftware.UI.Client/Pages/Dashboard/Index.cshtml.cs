using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PanacheSoftware.Core.Domain.API.Folder;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.API.Task;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Headers;
using PanacheSoftware.UI.Core.PageModel;

namespace PanacheSoftware.UI.Client.Pages.Dashboard
{
    [SecurityHeaders]
    [Authorize]
    [ValidateAntiForgeryToken]
    public class IndexModel : PageModel, IPanacheSoftwarePageModel
    {
        private readonly IAPIHelper _apiHelper;
        public LangQueryList langQueryList { get; set; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }
        public TaskGroupSummaryList taskGroupSummaryList { get; set; }
        public SaveMessageModel SaveMessageModel { get; set; }

        public IndexModel(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<bool> PageConstructor(string saveState, string accessToken)
        {
            SaveState = saveState;

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10121, 10112, 10113, 10807, 10408 });

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

            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.TASK, $"TaskGroup/GetTaskGroupSummary");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                taskGroupSummaryList = response.ContentAsType<TaskGroupSummaryList>();
            }
            else
            {
                taskGroupSummaryList = new TaskGroupSummaryList();
            }

            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken);

            return Page();
        }
    }
}
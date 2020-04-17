using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
//using Newtonsoft.Json;
using PanacheSoftware.Core.Domain.API.Client;
using PanacheSoftware.Core.Domain.API.Folder;
using PanacheSoftware.Core.Domain.API.Team;
using PanacheSoftware.Core.Domain.Identity.API;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Helpers;
using PanacheSoftware.Core.Extensions;
using PanacheSoftware.UI.Core.PageModel;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.API.Task;

namespace PanacheSoftware.UI.Client.Pages.TaskGroup
{
    public class IndexModel : PageModel, IPanacheSoftwarePageModel
    {
        private readonly IAPIHelper _apiHelper;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        public TaskGroupSummary taskGroupSummary { get; set; }
        public ClientSummary clientSummary { get; set; }
        public LangQueryList langQueryList { get; set; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }

        public IndexModel(IAPIHelper apiHelper, IRazorPartialToStringRenderer renderer, IModelHelper modelHelper, IMapper mapper)
        {
            _apiHelper = apiHelper;
            _renderer = renderer;
            _modelHelper = modelHelper;
            _mapper = mapper;
        }

        public async Task<bool> PageConstructor(string saveState, string accessToken)
        {
            SaveState = saveState;

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10121, 10111 });

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

            taskGroupSummary = new TaskGroupSummary();
            clientSummary = new ClientSummary();

            if (!string.IsNullOrWhiteSpace(Id))
            {
                var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.TASK, $"TaskGroup/GetTaskGroupSummary/{Id}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    taskGroupSummary = response.ContentAsType<TaskGroupSummary>();
                }
                else
                {
                    return RedirectToPage("/TaskGroup/TaskGroups");
                }
            }
            else
            {
                return RedirectToPage("/TaskGroup/TaskGroups");
            }

            if(taskGroupSummary != null)
            {
                var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.CLIENT, $"Client/GetClientSummary/{taskGroupSummary.ClientHeaderId}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    clientSummary = response.ContentAsType<ClientSummary>();
                }
            }

            return Page();
        }
    }
}
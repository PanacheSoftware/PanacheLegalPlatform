using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.API.Task;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Helpers;
using PanacheSoftware.UI.Core.PageModel;

namespace PanacheSoftware.UI.Client.Pages.TaskGroup
{
    public class CompleteModel : PageModel, IPanacheSoftwarePageModel
    {
        private readonly IAPIHelper _apiHelper;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        [BindProperty]
        public string taskGroupId { get; set; }
        [BindProperty]
        public string taskId { get; set; }
        public LangQueryList langQueryList { get; set; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }
        public SaveMessageModel SaveMessageModel { get; set; }

        public CompleteModel(IAPIHelper apiHelper, IRazorPartialToStringRenderer renderer, IModelHelper modelHelper, IMapper mapper)
        {
            _apiHelper = apiHelper;
            _renderer = renderer;
            _modelHelper = modelHelper;
            _mapper = mapper;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                await PageConstructor(SaveStates.IGNORE, accessToken);

                if (!await _apiHelper.AuthCheck(accessToken, User.FindFirst("sub").Value))
                {
                    return RedirectToPage("/Logout");
                }

                if (!string.IsNullOrWhiteSpace(taskGroupId))
                {
                    if (Guid.TryParse(taskGroupId, out Guid parsedId))
                    {
                        if (parsedId != Guid.Empty)
                        {
                            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.GATEWAY, $"TaskGroup/{parsedId}");

                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var taskGroupHead = response.ContentAsType<TaskGroupHead>();

                                if (!string.IsNullOrWhiteSpace(taskId))
                                {
                                    if (Guid.TryParse(taskGroupId, out Guid parsedTaskId))
                                    {
                                        if (parsedTaskId != Guid.Empty)
                                        {
                                            var childTask = taskGroupHead.ChildTasks.Where(c => c.Id == parsedTaskId).FirstOrDefault();

                                            if(childTask != null)
                                            {
                                                response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.GATEWAY, $"Task/Complete/{parsedTaskId}");

                                                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                                {
                                                    return RedirectToPage($"/TaskGroup/{taskGroupHead.Id}");
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.GATEWAY, $"TaskGroup/Complete/{parsedId}");

                                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                    {
                                        if (taskGroupHead.ParentTaskGroupId != null)
                                        {
                                            return RedirectToPage($"/TaskGroup/{taskGroupHead.ParentTaskGroupId}");
                                        }
                                        else
                                        {
                                            return RedirectToPage($"/TaskGroup/{taskGroupHead.Id}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    return RedirectToPage("/TaskGroup/TaskGroups");
                }

                return RedirectToPage("/TaskGroup/TaskGroups");
            }

            return RedirectToPage("/TaskGroup/TaskGroups");
        }

        public async Task<bool> PageConstructor(string saveState, string accessToken)
        {
            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.API.Team;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Helpers;
using PanacheSoftware.UI.Core.PageModel;

namespace PanacheSoftware.UI.Client.Pages.Team
{
    public class IndexModel : PageModel, IPanacheSoftwarePageModel
    {
        private readonly IAPIHelper _apiHelper;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        [BindProperty]
        public TeamHead teamHead { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        //public TeamHeaderModel teamHeaderModel { get; set; }
        public SelectList StatusList { get; }
        public SelectList TeamSelectList { get; set; }
        public string ChartDatasource { get; set; }
        public LangQueryList langQueryList { get; set; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }

        public IndexModel(IAPIHelper apiHelper, IRazorPartialToStringRenderer renderer, IModelHelper modelHelper, IMapper mapper)
        {
            _apiHelper = apiHelper;
            _renderer = renderer;
            _modelHelper = modelHelper;
            _mapper = mapper;

            StatusTypes statusTypes = new StatusTypes();
            StatusList = new SelectList(statusTypes.GetStatusTypesDictionary(), "Key", "Value");
            ChartDatasource = "{}";
        }

        public async Task<bool> PageConstructor(string saveState, string accessToken)
        {
            SaveState = saveState;

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10121, 10108, 10400, 10401, 10202, 10203, 10200, 10402, 10201, 10405, 10403, 10404, 10500, 10501 });

            await CreateTeamSelectList(accessToken);

            await GetTeamStructure(accessToken);

            return true;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            teamHead = new TeamHead();

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            if (!await _apiHelper.AuthCheck(accessToken, User.FindFirst("sub").Value))
            {
                return RedirectToPage("/Logout");
            }

            if (!string.IsNullOrWhiteSpace(Id))
            {
                var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.TEAM, $"Team/{Id}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    teamHead = response.ContentAsType<TeamHead>();
                }
            }
            else
            {
                Id = Guid.Empty.ToString();
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            await PageConstructor(SaveStates.IGNORE, accessToken);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            if (ModelState.IsValid)
            {
                if (teamHead != null)
                {
                    await CreateOrUpdateTeamAsync(accessToken);
                }

                await PageConstructor(SaveStates.SUCCESS, accessToken);

                return Page();
            }

            Id = teamHead.Id.ToString();

            await PageConstructor(SaveStates.FAILED, accessToken);

            return Page();
        }

        private async Task<bool> GetTeamStructure(string accessToken)
        {
            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.TEAM, $"Team/GetTeamStructure/{Id}");

            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                TeamStruct teamStruct = response.ContentAsType<TeamStruct>();

                TeamChartModel teamChartModel = _mapper.Map<TeamChartModel>(teamStruct);

                ChartDatasource = JsonConvert.SerializeObject(teamChartModel);
            }

            return true;
        }

        private async Task<bool> CreateTeamSelectList(string accessToken)
        {
            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.TEAM, $"Team/GetValidParents/{Id}");

            Dictionary<string, string> TeamListDictionary = new Dictionary<string, string>();
            TeamListDictionary.Add("", "None");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return false;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                TeamList teamList = response.ContentAsType<TeamList>();

                foreach (var teamHeader in teamList.TeamHeaders.OrderBy(h => h.ShortName))
                {
                    TeamListDictionary.Add(teamHeader.Id.ToString(), teamHeader.ShortName);
                }
            }

            TeamSelectList = new SelectList(TeamListDictionary, "Key", "Value");

            return true;
        }

        private async Task<bool> CreateOrUpdateTeamAsync(string apiAccessToken)
        {
            if (teamHead != null)
            {
                if (teamHead.Id != Guid.Empty)
                {
                    var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.TEAM, $"Team/{teamHead.Id}");

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return false;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var foundTeamHead = response.ContentAsType<TeamHead>();

                        if (foundTeamHead != null)
                        {
                            if (!await _modelHelper.ProcessPatch(foundTeamHead, teamHead, foundTeamHead.Id, apiAccessToken, APITypes.TEAM, "Team"))
                            {
                                return false;
                            }

                            response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.TEAM, $"TeamDetail/{teamHead.TeamDetail.Id}");
                            
                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var foundTeamDet = response.ContentAsType<TeamDet>();

                                if (foundTeamDet != null)
                                {
                                    if (!await _modelHelper.ProcessPatch(foundTeamDet, teamHead.TeamDetail, foundTeamDet.Id, apiAccessToken, APITypes.TEAM, "TeamDetail"))
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(teamHead), Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Post, APITypes.TEAM, $"Team", contentPost);

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
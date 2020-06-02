using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PanacheSoftware.Core.Domain.API.Join;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.API.Team;
using PanacheSoftware.Core.Domain.Identity;
using PanacheSoftware.Core.Domain.Identity.API;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Headers;
using PanacheSoftware.UI.Core.Helpers;
using PanacheSoftware.UI.Core.PageModel;

namespace PanacheSoftware.UI.Client.Pages.User
{
    [SecurityHeaders]
    [Authorize]
    [ValidateAntiForgeryToken]
    public class IndexModel : PageModel, IPanacheSoftwarePageModel
    {
        private readonly IMapper _mapper;
        private readonly IAPIHelper _apiHelper;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IModelHelper _modelHelper;

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
     
        [BindProperty]
        public UserProfileModel userProfileModel { get; set; }
        public UserTeamTableModel userTeamTableModel { get; set; }
        public SelectList StatusList { get; }
        public LangQueryList langQueryList { get; set; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }
        public SaveMessageModel SaveMessageModel { get; set; }

        public IndexModel(
            IMapper mapper,
            IAPIHelper apiHelper,
            IRazorPartialToStringRenderer renderer,
            IModelHelper modelHelper)
        {
            _mapper = mapper;
            _apiHelper = apiHelper;
            _renderer = renderer;
            _modelHelper = modelHelper;
            userProfileModel = new UserProfileModel();
            userTeamTableModel = new UserTeamTableModel();
            StatusTypes statusTypes = new StatusTypes();
            StatusList = new SelectList(statusTypes.GetStatusTypesDictionary(), "Key", "Value");
        }

        public async Task<bool> PageConstructor(string saveState, string accessToken)
        {
            SaveState = saveState;

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10121, 10115, 10500, 10501, 10801, 10800, 10802, 10803, 10804, 10207, 10209, 10202, 10203, 10205, 10221, 10200, 10201, 10805, 10806, 10406, 10400, 10407 });

            await CreateTeamSelectList(accessToken);
            await CreateUserTeamModelList(accessToken);
            await GeneratePageConstructionModel();

            userTeamTableModel.StatusList = StatusList;
            userTeamTableModel.userProfileModel = userProfileModel;
            userTeamTableModel.langQueryList = langQueryList;

            return true;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            if (!await _apiHelper.AuthCheck(accessToken, User.FindFirst("sub").Value))
            {
                return RedirectToPage("/Logout");
            }

            userProfileModel.userModel = new UserModel();
            userProfileModel.password = string.Empty;
            userProfileModel.passwordConfirm = string.Empty;

            if (!string.IsNullOrWhiteSpace(Id))
            {
                if(Guid.TryParse(Id, out Guid foundId))
                {
                    if (foundId != Guid.Empty)
                    {
                        var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.IDENTITY, $"User/{foundId.ToString()}");

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            userProfileModel.userModel = response.ContentAsType<UserModel>();
                        }
                    }
                }              
            }

            await PageConstructor(SaveStates.IGNORE, accessToken);

            //userTeamTableModel.StatusList = StatusList;
            //userTeamTableModel.userProfileModel = userProfileModel;

            //await CreateTeamSelectList(accessToken);
            //await CreateUserTeamModelList(accessToken);

            //await GeneratePageConstructionModel();

            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            if (ModelState.IsValid)
            {
                await PageConstructor(SaveStates.IGNORE, accessToken);

                if (userProfileModel.userModel.Id == Guid.Empty)
                {
                    var createUserModel = _mapper.Map<CreateUserModel>(userProfileModel.userModel);

                    createUserModel.Password = userProfileModel.password;
                    createUserModel.PasswordConfirm = userProfileModel.passwordConfirm;

                    string newUserContent = JsonConvert.SerializeObject(createUserModel);
                    HttpContent contentPost = new StringContent(newUserContent, Encoding.UTF8, "application/json");

                    var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Post, APITypes.IDENTITY, $"User", contentPost);

                    if(response.IsSuccessStatusCode)
                    {
                        if(await CreateOrUpdateUserTeamJoins(accessToken))
                        {
                            SaveState = SaveStates.SUCCESS;
                        }
                        else
                        {
                            SaveState = SaveStates.FAILED;
                        }
                    }
                }
                else
                {
                    //var accessToken = await HttpContext.GetTokenAsync("access_token");

                    if (await UpdateUser(accessToken))
                    {
                        SaveState = SaveStates.SUCCESS;
                    }
                    else
                    {
                        SaveState = SaveStates.FAILED;
                    }
                }
            }
            else
            {
                await PageConstructor(SaveStates.FAILED, accessToken);
                SaveState = SaveStates.FAILED;

                //userTeamTableModel.StatusList = StatusList;
                //userTeamTableModel.userProfileModel = userProfileModel;

                //await CreateTeamSelectList(accessToken);
                //await CreateUserTeamModelList(accessToken);

                //await GeneratePageConstructionModel();
            }

            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken, SaveState);

            return Page();
        }

        private async Task<bool> UpdateUser(string accessToken)
        {
            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.IDENTITY, $"User/{userProfileModel.userModel.Id.ToString()}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var foundUserModel = response.ContentAsType<UserModel>();

                if(foundUserModel != null)
                {
                    if(!await _modelHelper.ProcessPatch(foundUserModel, userProfileModel.userModel, foundUserModel.Id, accessToken, APITypes.IDENTITY, "User"))
                    {
                        return false;
                    }
                }
            }

            return await CreateOrUpdateUserTeamJoins(accessToken);
        }

        private async Task<bool> CreateOrUpdateUserTeamJoins(string accessToken)
        {
            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.GATEWAY, $"UserTeam/GetTeamsForUser/{userProfileModel.userModel.Id.ToString()}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                UserTeamJoinList userTeamJoinList = response.ContentAsType<UserTeamJoinList>();

                if (userTeamJoinList != null)
                {
                    foreach (var userTeamJoin in userProfileModel.userTeamJoins)
                    {
                        var foundUserTeamJoin = userTeamJoinList.UserTeamJoins.FirstOrDefault(utj => utj.Id == userTeamJoin.Id);

                        if (foundUserTeamJoin != null)
                        {
                            if (!await _modelHelper.ProcessPatch(foundUserTeamJoin, userTeamJoin, foundUserTeamJoin.Id, accessToken, APITypes.IDENTITY, "UserTeam"))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(userTeamJoin), Encoding.UTF8, "application/json");

                            try
                            {
                                response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Post, APITypes.GATEWAY, $"UserTeam", contentPost);

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
                }
            }

            return true;
        }

        private async Task<bool> CreateTeamSelectList(string accessToken)
        {
            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.GATEWAY, $"Team/");
            Dictionary<string, string> TeamListDictionary = new Dictionary<string, string>();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                TeamList teamList = response.ContentAsType<TeamList>();

                foreach (var teamHeader in teamList.TeamHeaders.OrderBy(h => h.ShortName))
                {
                    TeamListDictionary.Add(teamHeader.Id.ToString(), teamHeader.ShortName);
                }
            }

            userTeamTableModel.TeamSelectList = new SelectList(TeamListDictionary, "Key", "Value");

            return true;
        }

        private async Task<bool> CreateUserTeamModelList(string accessToken)
        {
            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.GATEWAY, $"UserTeam/GetTeamsForUser/{userProfileModel.userModel.Id}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                UserTeamJoinList userTeamJoinList = response.ContentAsType<UserTeamJoinList>();

                foreach (var userTeamJoin in userTeamJoinList.UserTeamJoins)
                {
                    var match = userProfileModel.userTeamJoins.Where(t => t.Id == userTeamJoin.Id).FirstOrDefault();
                    
                    if (match == null)
                    {
                        userProfileModel.userTeamJoins.Add(userTeamJoin);
                    }
                }
            }

            return true;
        }

        private async Task<bool> GeneratePageConstructionModel()
        {
            UserTeamTableModel dummyUserTeamTableModel = new UserTeamTableModel()
            {
                userProfileModel = GenerateDummyUserProfileModel(),
                StatusList = StatusList,
                TeamSelectList = userTeamTableModel.TeamSelectList
            };

            var teamListHTML = await _renderer.RenderPartialToStringAsync("_UserTeamList", dummyUserTeamTableModel);

            teamListHTML = teamListHTML.Replace("\n", "").Replace("\r", "").Replace("\t", "").ToString();

            var teamTableHTML = _renderer.GetStringBetween(teamListHTML, "<!--TeamTableRowsStart-->", "<!--TeamTableRowsEnd-->");

            List<string> teamListRows = teamTableHTML.TrimStart().TrimEnd().Replace("<td>", "").Split("</td>").ToList();

            for (int i = teamListRows.Count - 1; i >= 0; i--)
            {
                var eol = ",";

                teamListRows[i] = teamListRows[i].TrimStart().TrimEnd();
                if (string.IsNullOrWhiteSpace(teamListRows[i]))
                {
                    teamListRows.RemoveAt(i);
                    continue;
                }

                if (i == teamListRows.Count - 1)
                {
                    eol = "";
                }

                teamListRows[i] = teamListRows[i].Replace("userProfileModel_userTeamJoins_0__", "userProfileModel_userTeamJoins_' + rowCount + '__");
                teamListRows[i] = teamListRows[i].Replace("userProfileModel.userTeamJoins[0]", "userProfileModel.userTeamJoins[' + rowCount + ']");
                teamListRows[i] = teamListRows[i].Replace("22222222-2222-2222-2222-222222222222", "' + userId + '");
                teamListRows[i] = teamListRows[i].Replace("disabled", string.Empty); //Prevent the team drop down from being disabled on new rows

                teamListRows[i] = $"'{teamListRows[i]}'{eol}";
            }

            userTeamTableModel.teamListRows = teamListRows.ToArray();

            return true;
        }

        private UserProfileModel GenerateDummyUserProfileModel()
        {
            var dummyUserTeamModel = GenerateDummyUserTeamModel();

            var dummyUserProfileModel = new UserProfileModel()
            {
                userModel = new UserModel(),
                userTeamJoins = new List<UserTeamJoin>()
            };

            dummyUserProfileModel.userTeamJoins.Add(GenerateDummyUserTeamModel());

            return dummyUserProfileModel;
        }

        private UserTeamJoin GenerateDummyUserTeamModel()
        {
            var dummyUserTeamModel = new UserTeamJoin()
            {
                Id = Guid.Empty,
                TeamHeaderId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddYears(1),
                Status = StatusTypes.Open
            };

            return dummyUserTeamModel;
        }
    }
}
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.API.Team;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.PageModel;

namespace PanacheSoftware.UI.Client.Pages.Team
{
    [Authorize]
    public class TeamsModel : PageModel, IPanacheSoftwarePageModel
    {
        private readonly IAPIHelper _apiHelper;
        public TeamList teamList { get; set; }
        public LangQueryList langQueryList { get; set; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }
        public SaveMessageModel SaveMessageModel { get; set; }

        public TeamsModel(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<bool> PageConstructor(string saveState, string accessToken)
        {
            SaveState = saveState;

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10121, 10108, 10400, 10401, 10200, 10201, 10202, 10203, 10204 });

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

            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.TEAM, $"Team");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                teamList = response.ContentAsType<TeamList>();
            }
            else
            {
                teamList = new TeamList();
            }

            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken);

            return Page();
        }
    }
}
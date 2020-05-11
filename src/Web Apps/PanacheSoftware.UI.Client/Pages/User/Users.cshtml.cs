using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.Identity;
using PanacheSoftware.Core.Domain.Identity.API;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Headers;
using PanacheSoftware.UI.Core.PageModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PanacheSoftware.UI.Client.Pages.User
{
    [SecurityHeaders]
    [Authorize]
    [ValidateAntiForgeryToken]
    public class UsersModel : PageModel, IPanacheSoftwarePageModel
    {
        private readonly IAPIHelper _apiHelper;
        private readonly IMapper _mapper;

        public UsersViewModel usersViewModel { get; set; }
        public LangQueryList langQueryList { get; set; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }
        public SaveMessageModel SaveMessageModel { get; set; }

        public UsersModel(
            IAPIHelper apiHelper,
            IMapper mapper)
        {
            _apiHelper = apiHelper;
            _mapper = mapper;
        }
        public async Task<bool> PageConstructor(string saveState, string accessToken)
        {
            SaveState = saveState;

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10121, 10115, 10800, 10207, 10209, 10201, 10202, 10203, 10204 });

            return true;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            usersViewModel = new UsersViewModel();

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            await PageConstructor(SaveStates.IGNORE, accessToken);

            if (!await _apiHelper.AuthCheck(accessToken, User.FindFirst("sub").Value))
            {
                return RedirectToPage("/Logout");
            }

            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.IDENTITY, $"User");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var userListModel = response.ContentAsType<UserListModel>();

                usersViewModel = _mapper.Map<UsersViewModel>(userListModel);
            }

            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken);

            return Page();
        }

        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Headers;
using PanacheSoftware.UI.Core.PageModel;

namespace PanacheSoftware.UI.Client.Pages.User
{
    [SecurityHeaders]
    [Authorize]
    [ValidateAntiForgeryToken]
    public class ProfileModel : PageModel, IPanacheSoftwarePageModel
    {
        private readonly IAPIHelper _apiHelper;
        private readonly IMapper _mapper;

        public LangQueryList langQueryList { get; set; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }
        public SaveMessageModel SaveMessageModel { get; set; }
        public AuthTokenModel AuthTokenModel { get; set; }

        public ProfileModel(
            IAPIHelper apiHelper,
            IMapper mapper)
        {
            _apiHelper = apiHelper;
            _mapper = mapper;
        }

        public async Task<bool> PageConstructor(string saveState, string accessToken)
        {
            SaveState = saveState;

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10121 });

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

            AuthTokenModel = new AuthTokenModel(accessToken);

            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken);

            return Page();
        }
    }

    public class AuthTokenModel
    {
        public string AuthToken { get; }
        public AuthTokenModel(string authToken)
        {
            AuthToken = authToken;
        }
    }
}
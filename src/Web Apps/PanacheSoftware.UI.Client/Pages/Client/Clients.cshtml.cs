using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PanacheSoftware.Core.Domain.API.Client;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Headers;
using PanacheSoftware.UI.Core.PageModel;

namespace PanacheSoftware.UI.Client.Pages.Client
{
    [SecurityHeaders]
    [Authorize]
    [ValidateAntiForgeryToken]
    public class ClientsModel : PageModel, IPanacheSoftwarePageModel
    {
        private readonly IAPIHelper _apiHelper;
        public ClientList clientList { get; set; }
        public LangQueryList langQueryList { get; set; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }
        public SaveMessageModel SaveMessageModel { get; set; }

        public ClientsModel(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<bool> PageConstructor(string saveState, string accessToken)
        {
            SaveState = saveState;

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10121, 10105, 10301, 10302, 10200, 10201, 10202, 10203, 10204 });

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

            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.CLIENT, $"Client");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                clientList = response.ContentAsType<ClientList>();
            }
            else
            {
                clientList = new ClientList();
            }

            //var response2 = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.GATEWAY, $"Client");

            //if (response2.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    var clientList2 = response.ContentAsType<ClientList>();
            //}


            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken);

            return Page();
        }
    }
}
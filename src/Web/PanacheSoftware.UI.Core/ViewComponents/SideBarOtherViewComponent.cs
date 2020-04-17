using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Headers;
using System.Threading.Tasks;

namespace PanacheSoftware.UI.Core.ViewComponents
{
    [SecurityHeaders]
    [Authorize]
    [ValidateAntiForgeryToken]
    public class SideBarOtherViewComponent : ViewComponent
    {
        private readonly IAPIHelper _apiHelper;

        public SideBarOtherViewComponent(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var languageSetting = await _apiHelper.GetUserLanguage(accessToken);

            var langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, languageSetting.Value, new long[] {  });

            return View(langQueryList);
        }
    }
}

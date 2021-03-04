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
    public class SideBarViewComponent : ViewComponent
    {
        private readonly IAPIHelper _apiHelper;

        public SideBarViewComponent(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var languageSetting = await _apiHelper.GetUserLanguage(accessToken);

            var langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, languageSetting.Value, new long[] { 10000, 10102, 10103, 10104, 10105, 10106, 10107, 10108, 10109, 10100, 10110, 10111, 10112, 10113, 10114, 10115, 10116, 10117, 10118, 10119, 10122, 10123, 10124, 10125, 10126 });

            return View(langQueryList);
        }
    }
}

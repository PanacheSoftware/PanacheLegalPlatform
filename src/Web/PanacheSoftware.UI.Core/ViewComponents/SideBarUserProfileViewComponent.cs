using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PanacheSoftware.Core.Domain.Configuration;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Headers;

namespace PanacheSoftware.UI.Core.ViewComponents
{
    [SecurityHeaders]
    [Authorize]
    [ValidateAntiForgeryToken]
    public class SideBarUserProfileViewComponent : ViewComponent
    {
        private readonly IAPIHelper _apiHelper;
        private readonly IConfiguration _configuration;

        public SideBarUserProfileViewComponent(IAPIHelper apiHelper, IConfiguration configuration)
        {
            _apiHelper = apiHelper;
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var languageSetting = await _apiHelper.GetUserLanguage(accessToken);

            var langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, languageSetting.Value, new long[] { 10100 });

            var panacheSoftwareConfiguration = new PanacheSoftwareConfiguration();
            _configuration.Bind("PanacheSoftware", panacheSoftwareConfiguration);

            var discoveryClient = new HttpClient();
            //var doc = await discoveryClient.GetDiscoveryDocumentAsync("https://localhost:44302");
            var doc = await discoveryClient.GetDiscoveryDocumentAsync(bool.Parse(panacheSoftwareConfiguration.CallMethod.UICallsSecure) ? panacheSoftwareConfiguration.Url.IdentityServerURLSecure : panacheSoftwareConfiguration.Url.IdentityServerURL);

            var userInfoClient = new HttpClient();
            var userInfoResponse = await userInfoClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = doc.UserInfoEndpoint,
                Token = accessToken
            });

            if (userInfoResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                var name = userInfoResponse.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Name);
                var picture = userInfoResponse.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Picture);
                var id = userInfoResponse.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject);

                ViewData["FullName"] = name.Value;
                ViewData["Picture"] = picture.Value;
                ViewData["Id"] = id.Value;
            }
            else
            {
                ViewData["FullName"] = string.Empty;
                ViewData["Picture"] = Base64Images.PanacheSoftwareDot;
                ViewData["Id"] = string.Empty;
            }

            return View(langQueryList);
        }
    }
}


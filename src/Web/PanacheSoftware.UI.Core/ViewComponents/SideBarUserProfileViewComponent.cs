using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public SideBarUserProfileViewComponent(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var languageSetting = await _apiHelper.GetUserLanguage(accessToken);

            var langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, languageSetting.Value, new long[] { 10100 });

            //var idToken = await HttpContext.GetTokenAsync("id_token");

            //var client = new HttpClient();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            //var content1 = await client.GetStringAsync("https://localhost:44397/api/identity");
            //var content2 = await client.GetStringAsync("https://localhost:44397/.well-known/openid-configuration");

            //var clientID = new HttpClient();
            //clientID.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", idToken);
            //var content3 = await clientID.GetStringAsync("https://localhost:44397/connect/userinfo");

            var discoveryClient = new HttpClient();// new DiscoveryClient("https://localhost:44397");
            var doc = await discoveryClient.GetDiscoveryDocumentAsync("https://localhost:44397");

            //var tokenEndpoint = doc.TokenEndpoint;
            //var keys = doc.KeySet.Keys;

            //var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            //var rt = await HttpContext.GetTokenAsync("refresh_token");
            //var tokenClient = new HttpClient();

            //var tokenResult = await tokenClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            //{
            //    Address = doc.TokenEndpoint,

            //    ClientId = PanacheSoftwareScopeNames.ClientUI,
            //    ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0",
            //    RefreshToken = rt
            //});

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

            //var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            //ViewData["FullName"] = currentUser.FullName;
            //ViewData["Picture"] = currentUser.Base64ProfileImage;

            return View(langQueryList);
        }
    }
}


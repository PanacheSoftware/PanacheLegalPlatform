using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Types;
using PanacheSoftware.UI.Core.Headers;

namespace PanacheSoftware.UI.Core.ViewComponents
{
    [SecurityHeaders]
    [Authorize]
    [ValidateAntiForgeryToken]
    public class SideBarUserProfileViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(LangQueryList sideBarQueryList)
        {
            var name = ((ClaimsIdentity)User.Identity).FindFirst(JwtClaimTypes.Name);
            var picture = ((ClaimsIdentity)User.Identity).FindFirst(JwtClaimTypes.Picture);
            var id = ((ClaimsIdentity)User.Identity).FindFirst(JwtClaimTypes.Subject);

            if (name != null)
            {
                ViewData["FullName"] = name.Value;
            }
            else
            {
                ViewData["FullName"] = string.Empty;
            }

            if (picture != null)
            {
                ViewData["Picture"] = picture.Value;
            }
            else
            {
                ViewData["Picture"] = Base64Images.PanacheSoftwareDot;
            }

            if (id != null)
            {
                ViewData["Id"] = id.Value;
            }
            else
            {
                ViewData["Id"] = string.Empty;
            }

            return View(sideBarQueryList);
        }
    }
}


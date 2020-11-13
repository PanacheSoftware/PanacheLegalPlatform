using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.UI.Core.Headers;
using System.Threading.Tasks;
using PanacheSoftware.Core.Domain.API.Language;

namespace PanacheSoftware.UI.Core.ViewComponents
{
    [SecurityHeaders]
    [Authorize]
    [ValidateAntiForgeryToken]
    public class SideBarUserViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(LangQueryList sideBarQueryList)
        {
            return View(sideBarQueryList);
        }
    }
}

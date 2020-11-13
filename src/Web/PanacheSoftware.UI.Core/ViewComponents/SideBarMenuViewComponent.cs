using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.UI.Core.Headers;

namespace PanacheSoftware.UI.Core.ViewComponents
{
    [SecurityHeaders]
    [Authorize]
    [ValidateAntiForgeryToken]
    public class SideBarMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(LangQueryList sideBarQueryList)
        {
            return View(sideBarQueryList);
        }
    }
}
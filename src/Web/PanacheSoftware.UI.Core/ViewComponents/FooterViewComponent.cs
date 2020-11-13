using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.Language;

namespace PanacheSoftware.UI.Core.ViewComponents
{
    [Authorize]
    public class FooterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(LangQueryList sideBarQueryList)
        {
            return View(sideBarQueryList);
        }
    }
}
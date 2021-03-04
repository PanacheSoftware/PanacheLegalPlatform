using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.UI.Core.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.UI.Core.ViewComponents
{
    [SecurityHeaders]
    [Authorize]
    [ValidateAntiForgeryToken]
    public class SideBarCustomFieldViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(LangQueryList sideBarQueryList)
        {
            return View(sideBarQueryList);
        }
    }
}

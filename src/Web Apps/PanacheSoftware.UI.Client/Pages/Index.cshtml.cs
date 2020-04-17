using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PanacheSoftware.UI.Core.Headers;

namespace PanacheSoftware.UI.Client.Pages
{
    [SecurityHeaders]
    [Authorize]
    [ValidateAntiForgeryToken]
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            return new RedirectToPageResult("Dashboard/Index");
        }
    }
}

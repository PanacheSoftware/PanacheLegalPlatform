using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PanacheSoftware.Http;

namespace PanacheSoftware.Identity.Pages
{
    public class IndexModel : PageModel
    {
        public string clientURL { get; set; }

        private readonly IAPIHelper _apiHelper;

        public IndexModel(
            IAPIHelper apihelper)
        {
            _apiHelper = apihelper;
        }

        public IActionResult OnGet()
        {
            clientURL = _apiHelper.GetBaseURL(APITypes.UICLIENT);
            return Page();
            //return new RedirectToPageResult("User/Users");
        }
    }
}

using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Duende.IdentityServer.IdentityServerConstants;

namespace PanacheSoftware.Identity.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(LocalApi.PolicyName)]
    public class IdentityController : ControllerBase
    {
        //private readonly UserManager<ApplicationUser> _userManager;

        //[HttpGet]
        //public IActionResult Get()
        //{
        //    return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        //}
    }
}
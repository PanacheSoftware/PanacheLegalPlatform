using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace PanacheSoftware.Identity.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(LocalApi.PolicyName)]
    public class LoginController : ControllerBase
    {
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAsync()
        {
            return NotFound();
        }
    }
}

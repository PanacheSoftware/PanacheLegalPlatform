using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using System.Security.Claims;

namespace PanacheSoftware.Http
{
    public class UserProvider : IUserProvider
    {
        private IHttpContextAccessor _accessor;

        public UserProvider(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string GetTenantId()
        {
            if (_accessor.HttpContext != null)
            {
                if (_accessor.HttpContext.User != null)
                {
                    var curUsr = _accessor.HttpContext.User;
                    if (curUsr.Identity != null)
                    {
                        return curUsr.FindFirst("tenantid").Value;
                    }
                }
            }
            return Guid.Empty.ToString();
        }

        public string GetUserId()
        {
            //User.FindFirstValue(ClaimTypes.NameIdentifier);
            var curUsr = _accessor.HttpContext.User;
            if (curUsr.Identity != null)
            {
                return curUsr.FindFirst("sub").Value;
            }
            return Guid.Empty.ToString();
        }
    }
}

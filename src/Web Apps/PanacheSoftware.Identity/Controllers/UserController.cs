using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Identity;
using PanacheSoftware.Core.Domain.Identity.API;
using PanacheSoftware.Http;
using PanacheSoftware.Identity.Manager;
using static IdentityServer4.IdentityServerConstants;

namespace PanacheSoftware.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(LocalApi.PolicyName)]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IApplicationUserManager _applicationUserManager;
        private readonly IUserProvider _userProvider;

        public UserController(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IApplicationUserManager applicationUserManager,
            IUserProvider userProvider)
        {
            _userManager = userManager;
            _mapper = mapper;
            _applicationUserManager = applicationUserManager;
            _userProvider = userProvider;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                UserListModel userList = new UserListModel();

                Claim tenantClaim = new Claim("tenantid", _userProvider.GetTenantId());

                var foundUsers = await _userManager.GetUsersForClaimAsync(tenantClaim);

                //foreach (var foundUser in _userManager.Users.AsNoTracking())
                foreach (var foundUser in foundUsers)
                {
                    userList.Users.Add(_mapper.Map<UserModel>(foundUser));
                }

                if (userList.Users.Any())
                    return Ok(userList);
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAsync(string id)
        {
            try
            {
                ApplicationUser applicationUser = null;

                if(Guid.TryParse(id, out Guid parsedId))
                {
                    applicationUser = await _userManager.FindByIdAsync(parsedId.ToString());
                }
                else
                {
                    applicationUser = await _userManager.FindByNameAsync(id);
                }

                if(applicationUser != null)
                {
                    return Ok(_mapper.Map<UserModel>(applicationUser));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody]CreateUserModel createUserModel)
        {
            try
            {
                if (createUserModel.Id == Guid.Empty)
                {
                    var userModel = _mapper.Map<UserModel>(createUserModel);

                    var result = await _applicationUserManager.CreateUserAsync(userModel, ModelState, createUserModel.Password, createUserModel.PasswordConfirm, Guid.Parse(_userProvider.GetTenantId()));

                    if (result.Succeeded)
                    {
                        return Created(new Uri($"https://localhost:44380/User/{userModel.Id}", UriKind.Absolute), userModel);
                    }
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return BadRequest();
        }

        [HttpPatch("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> PatchAsync(string id, [FromBody]JsonPatchDocument<UserModel> userModelPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    var applicationUser = await _userManager.FindByIdAsync(parsedId.ToString());

                    var userModel = _mapper.Map<UserModel>(applicationUser);

                    userModelPatch.ApplyTo(userModel);

                    _mapper.Map(userModel, applicationUser);

                    applicationUser.SecurityStamp = new Guid().ToString();
                    var updateUserResult = await _userManager.UpdateAsync(applicationUser);

                    if (updateUserResult.Succeeded)
                    {
                        return Created(new Uri($"https://localhost:44380/User/{userModel.Id}", UriKind.Absolute), userModel);
                    }
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return BadRequest();
        }
    }
}
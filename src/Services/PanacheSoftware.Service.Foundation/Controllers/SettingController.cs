using System;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.Error;
using PanacheSoftware.Core.Domain.API.Settings;
using PanacheSoftware.Core.Domain.Settings;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Service.Foundation.Core;

namespace PanacheSoftware.Service.Foundation.Controllers
{
    [Produces("application/json")]
    [Authorize]
    [Route("[controller]")]
    public class SettingController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SettingController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public IActionResult Get()
        {
            try
            {
                TenantSettingList tenantSettingList = new TenantSettingList();

                foreach (var currentTenantSetting in _unitOfWork.TenantSettings.GetTenantSettings())
                {
                    tenantSettingList.TenantSettings.Add(_mapper.Map<TenSetting>(currentTenantSetting));
                }

                if (tenantSettingList.TenantSettings.Count > 0)
                    return Ok(tenantSettingList);

                return NotFound();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public IActionResult Get(string id)
        {
            try
            {
                TenantSetting tenantSetting;

                if (Guid.TryParse(id, out Guid foundId))
                {
                    tenantSetting = _unitOfWork.TenantSettings.GetTenantSetting(foundId);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        tenantSetting = _unitOfWork.TenantSettings.GetTenantSetting(id);
                    }
                    else
                    {
                        tenantSetting = null;
                    }
                }

                if (tenantSetting != null)
                {
                    return Ok(_mapper.Map<TenSetting>(tenantSetting));
                }

                return NotFound();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] TenSetting tenSetting)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (tenSetting.Id == Guid.Empty)
                    {
                        var foundExisting = _unitOfWork.TenantSettings.GetTenantSetting(tenSetting.Id);

                        if (foundExisting == null)
                        {
                            var tenantSetting = _mapper.Map<TenantSetting>(tenSetting);
                            tenantSetting.Status = StatusTypes.Open;

                            _unitOfWork.TenantSettings.Add(tenantSetting);

                            _unitOfWork.Complete();

                            return Created(new Uri($"{Request.Path}/{tenantSetting.Id}", UriKind.Absolute),
                                _mapper.Map<TenSetting>(tenantSetting));
                        }

                        return StatusCode(StatusCodes.Status400BadRequest,
                            new APIErrorMessage(StatusCodes.Status400BadRequest,
                                $"TenSetting.Id: '{tenSetting.Id}' already exists."));

                    }

                    return StatusCode(StatusCodes.Status400BadRequest,
                        new APIErrorMessage(StatusCodes.Status400BadRequest,
                            $"TenSetting.Id: '{tenSetting.Id}' is not an empty guid."));
                }
                catch (Exception e)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
                }
            }

            return BadRequest(new APIErrorMessage(StatusCodes.Status400BadRequest, "One or more validation errors occurred.", ModelState));
        }

        [HttpPatch("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<TenSetting> tenantSettingPatch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Guid.TryParse(id, out Guid parsedId))
                    {
                        TenantSetting tenantSetting = _unitOfWork.TenantSettings.Get(parsedId);

                        if (tenantSetting != null)
                        {
                            TenSetting tenSetting = _mapper.Map<TenSetting>(tenantSetting);

                            tenantSettingPatch.ApplyTo(tenSetting);

                            _mapper.Map(tenSetting, tenantSetting);

                            _unitOfWork.Complete();

                            return CreatedAtRoute("Get", new {id = _mapper.Map<TenSetting>(tenantSetting).Id},
                                _mapper.Map<TenSetting>(tenantSetting));
                        }

                        return NotFound();
                    }

                    return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"id: '{id}' is not a valid guid."));
                }
                catch (Exception e)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
                }
            }

            return BadRequest(new APIErrorMessage(StatusCodes.Status400BadRequest, "One or more validation errors occurred.", ModelState));
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult UserSetting()
        {
            try
            {
                UsrSettingList usrSettingList = new UsrSettingList();

                var userId = User.FindFirstValue("sub");

                foreach (var currentUserSetting in _unitOfWork.UserSettings.GetUserSettings(Guid.Parse(userId)))
                {
                    usrSettingList.UserSettings.Add(_mapper.Map<UsrSetting>(currentUserSetting));
                }

                if (usrSettingList.UserSettings.Count > 0)
                    return Ok(usrSettingList);

                return NotFound();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public IActionResult UserSetting(string id)
        {
            try
            {
                UserSetting userSetting;
                var userId = User.FindFirstValue("sub");

                if (Guid.TryParse(id, out Guid foundId))
                {
                    userSetting = _unitOfWork.UserSettings.GetUserSetting(foundId, Guid.Parse(userId));
                }
                else
                {
                    userSetting = _unitOfWork.UserSettings.GetUserSetting(id, Guid.Parse(userId));
                }

                if (userSetting != null)
                {
                    return Ok(_mapper.Map<UsrSetting>(userSetting));
                }

                return NotFound();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult UserSetting([FromBody]UsrSetting usrSetting)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (usrSetting.Id == Guid.Empty && usrSetting.SettingHeaderId != Guid.Empty)
                    {
                        var userId = User.FindFirstValue("sub");

                        var foundSettingHeader = _unitOfWork.SettingHeaders.GetSettingHeader(usrSetting.SettingHeaderId,
                            settingType: SettingTypes.USER, includeUser: false);

                        var foundExisting =
                            _unitOfWork.UserSettings.GetUserSetting(foundSettingHeader.Name, Guid.Parse(userId));

                        if (foundExisting.Id == Guid.Empty)
                        {
                            var userSetting = _mapper.Map<UserSetting>(usrSetting);

                            //Make sure you can only create user settings for current user, this should be fixed to allow for admin created values
                            userSetting.UserId = Guid.Parse(userId);
                            userSetting.Status = StatusTypes.Open; //Shouldn't do this here!

                            _unitOfWork.UserSettings.Add(userSetting);

                            _unitOfWork.Complete();

                            return Created(new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}/{userSetting.Id}", UriKind.Relative),
                                _mapper.Map<UsrSetting>(userSetting));
                        }

                        return StatusCode(StatusCodes.Status400BadRequest,
                            new APIErrorMessage(StatusCodes.Status400BadRequest,
                                $"User setting already exists."));
                    }

                    return StatusCode(StatusCodes.Status400BadRequest,
                        new APIErrorMessage(StatusCodes.Status400BadRequest,
                            $"UsrSetting.Id: '{usrSetting.Id}' must be empty. UsrSetting.SettingHeaderId: must not be an empty guid."));
                }
                catch (Exception e)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
                }
            }

            return BadRequest(new APIErrorMessage(StatusCodes.Status400BadRequest, "One or more validation errors occurred.", ModelState));
        }

        [Route("[action]/{id}")]
        [HttpPatch]
        public IActionResult UserSetting(string id, [FromBody]JsonPatchDocument<UsrSetting> usrSettingPatch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Guid.TryParse(id, out Guid parsedId))
                    {
                        var userId = User.FindFirstValue("sub");

                        UserSetting userSetting = _unitOfWork.UserSettings.Get(parsedId);

                        if (userSetting != null)
                        {
                            UsrSetting usrSetting = _mapper.Map<UsrSetting>(userSetting);

                            usrSettingPatch.ApplyTo(usrSetting);

                            _mapper.Map(usrSetting, userSetting);

                            _unitOfWork.Complete();

                            return Ok(_mapper.Map<UsrSetting>(userSetting));
                        }

                        return NotFound();
                    }

                    return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"id: '{id}' is not a valid guid."));
                }
                catch (Exception e)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
                }
            }

            return BadRequest(new APIErrorMessage(StatusCodes.Status400BadRequest, "One or more validation errors occurred.", ModelState));
        }
    }
}
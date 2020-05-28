using System;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.Settings;
using PanacheSoftware.Core.Domain.Settings;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Service.Foundation.Core;

namespace PanacheSoftware.Service.Foundation.Controllers
{
    [Produces("application/json")]
    [Authorize]
    [Route("api/[controller]")]
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
                SettingList settingList = new SettingList();

                foreach (var currentSetting in _unitOfWork.SettingHeaders.GetSettingHeaders(includeUser: false))
                {
                    settingList.SettingHeaders.Add(_mapper.Map<SettingHead>(currentSetting));
                }

                if (settingList.SettingHeaders.Count > 0)
                    return Ok(settingList);
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
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public IActionResult Get(string id)
        {
            try
            {
                SettingHeader settingHeader;

                if (Guid.TryParse(id, out Guid foundId))
                {
                    settingHeader = _unitOfWork.SettingHeaders.GetSettingHeader(settingHeaderId: foundId, includeUser: false);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        settingHeader = _unitOfWork.SettingHeaders.GetSettingHeader(settingHeaderName: id, includeUser: false);
                    }
                    else
                    {
                        settingHeader = null;
                    }
                }

                if (settingHeader != null)
                {
                    return Ok(_mapper.Map<SettingHead>(settingHeader));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Post([FromBody]SettingHead settingHead)
        {
            try
            {
                if (settingHead.Id == Guid.Empty)
                {
                    var foundExisting = _unitOfWork.SettingHeaders.GetSettingHeader(settingHead.Id, includeUser: false);

                    if (foundExisting == null)
                    {
                        //var userId = User.FindFirstValue("sub");

                        var settingHeader = _mapper.Map<SettingHeader>(settingHead);

                        _unitOfWork.SettingHeaders.Add(settingHeader);

                        _unitOfWork.Complete();

                        return Created(new Uri($"{Request.Path}/{settingHeader.Id}", UriKind.Relative), _mapper.Map<SettingHead>(settingHeader));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<SettingHead> settingHeadPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    //var userId = User.FindFirstValue("sub");

                    SettingHeader settingHeader = _unitOfWork.SettingHeaders.Get(parsedId);

                    SettingHead settingHead = _mapper.Map<SettingHead>(settingHeader);

                    settingHeadPatch.ApplyTo(settingHead);

                    _mapper.Map(settingHead, settingHeader);

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Get", new { id = _mapper.Map<SettingHead>(settingHeader).Id }, _mapper.Map<SettingHead>(settingHeader));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return BadRequest();
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
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
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
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult UserSetting([FromBody]UsrSetting usrSetting)
        {
            try
            {
                if (usrSetting.Id == Guid.Empty && usrSetting.SettingHeaderId != Guid.Empty)
                {
                    var userId = User.FindFirstValue("sub");

                    var foundSettingHeader = _unitOfWork.SettingHeaders.GetSettingHeader(usrSetting.SettingHeaderId, settingType: SettingTypes.USER, includeUser: false);

                    var foundExisting = _unitOfWork.UserSettings.GetUserSetting(foundSettingHeader.Name, Guid.Parse(userId));

                    if (foundExisting.Id == Guid.Empty)
                    {
                        var userSetting = _mapper.Map<UserSetting>(usrSetting);

                        //Make sure you can only create user settings for current user, this should be fixed to allow for admin created values
                        userSetting.UserId = Guid.Parse(userId);
                        userSetting.Status = StatusTypes.Open; //Shouldn't do this here!

                        _unitOfWork.UserSettings.Add(userSetting);

                        _unitOfWork.Complete();

                        return Created(new Uri($"{Request.Path}/{userSetting.Id}", UriKind.Relative), _mapper.Map<UsrSetting>(userSetting));
                    }
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return BadRequest();
        }

        [Route("[action]/{id}")]
        [HttpPatch]
        public IActionResult UserSetting(string id, [FromBody]JsonPatchDocument<UsrSetting> usrSettingPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    var userId = User.FindFirstValue("sub");

                    UserSetting userSetting = _unitOfWork.UserSettings.Get(parsedId);

                    UsrSetting usrSetting = _mapper.Map<UsrSetting>(userSetting);

                    usrSettingPatch.ApplyTo(usrSetting);

                    _mapper.Map(usrSetting, userSetting);

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Get", new { id = _mapper.Map<UsrSetting>(userSetting).Id }, _mapper.Map<UsrSetting>(userSetting));
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
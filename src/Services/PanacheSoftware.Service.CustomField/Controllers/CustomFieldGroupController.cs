using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.CustomField;
using PanacheSoftware.Core.Domain.API.Error;
using PanacheSoftware.Core.Domain.CustomField;
using PanacheSoftware.Service.CustomField.Core;
using PanacheSoftware.Service.CustomField.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.CustomField.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class CustomFieldGroupController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICustomFieldManager _customFieldManager;

        public CustomFieldGroupController(IUnitOfWork unitOfWork, IMapper mapper, ICustomFieldManager customFieldManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _customFieldManager = customFieldManager;
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
                CustomFieldGroupList customFieldGroupList = new CustomFieldGroupList();

                foreach (var customFieldGroupHeader in _unitOfWork.CustomFieldGroupHeaders.GetAll(true))
                {
                    customFieldGroupList.CustomFieldGroupHeaders.Add(_mapper.Map<CustomFieldGroupHead>(customFieldGroupHeader));
                }

                if (customFieldGroupList.CustomFieldGroupHeaders.Count > 0)
                    return Ok(customFieldGroupList);

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
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");

                    var customFieldGroupHeader = _unitOfWork.CustomFieldGroupHeaders.GetCustomFieldGroupHeaderWithRelations(parsedId, true);

                    if (customFieldGroupHeader != default)
                    {
                        return Ok(_mapper.Map<CustomFieldGroupHead>(customFieldGroupHeader));
                    }

                    return NotFound();
                }

                return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"id: '{id}' is not a valid guid."));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CustomFieldGroupHead customFieldGroupHead)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");

                    if (customFieldGroupHead.Id == Guid.Empty)
                    {
                        if(_customFieldManager.CustomFieldGroupShortNameExists(customFieldGroupHead.ShortName))
                            return StatusCode(StatusCodes.Status400BadRequest,
                                new APIErrorMessage(StatusCodes.Status400BadRequest,
                                    $"CustomFieldGroupHead.ShortName: '{customFieldGroupHead.ShortName}' already exists."));

                        _customFieldManager.SetCustomFieldShortNames(customFieldGroupHead);

                        if(_customFieldManager.BlankShortNames(customFieldGroupHead.CustomFieldHeaders))
                            return StatusCode(StatusCodes.Status400BadRequest,
                                new APIErrorMessage(StatusCodes.Status400BadRequest,
                                    $"CustomFieldGroupHead.CustomFieldHeaders: Error setting Field ShortNames, cannot be blank."));

                        var duplicateShortNames = _customFieldManager.DuplicateShortNames(customFieldGroupHead);

                        if(duplicateShortNames.Count > 0)
                            return StatusCode(StatusCodes.Status400BadRequest,
                                new APIErrorMessage(StatusCodes.Status400BadRequest,
                                    $"CustomFieldGroupHead.CustomFieldHeaders: Duplicate ShortNames exist - {string.Join(",", duplicateShortNames)}"));

                        var customFieldGroupHeader = _mapper.Map<CustomFieldGroupHeader>(customFieldGroupHead);

                        _unitOfWork.CustomFieldGroupHeaders.Add(customFieldGroupHeader);

                        _unitOfWork.Complete();

                        return Created(new Uri($"{Request.Path}/{customFieldGroupHeader.Id}", UriKind.Relative),
                            _mapper.Map<CustomFieldGroupHead>(customFieldGroupHeader));
                    }

                    return StatusCode(StatusCodes.Status400BadRequest,
                        new APIErrorMessage(StatusCodes.Status400BadRequest,
                            $"CustomFieldGroupHead.Id: '{customFieldGroupHead.Id}' is not an empty guid."));
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
        public IActionResult Patch(string id, [FromBody] JsonPatchDocument<CustomFieldGroupHead> customFieldGroupHeadPatch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Guid.TryParse(id, out Guid parsedId))
                    {
                        CustomFieldGroupHeader customFieldGroupHeader = _unitOfWork.CustomFieldGroupHeaders.Get(parsedId);

                        if (customFieldGroupHeader != null)
                        {
                            var customFieldGroupHead = _mapper.Map<CustomFieldGroupHead>(customFieldGroupHeader);

                            customFieldGroupHeadPatch.ApplyTo(customFieldGroupHead);

                            _mapper.Map(customFieldGroupHead, customFieldGroupHeader);

                            _unitOfWork.Complete();

                            return Ok(_mapper.Map<CustomFieldGroupHead>(customFieldGroupHeader));
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

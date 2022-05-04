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
    public class CustomFieldController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICustomFieldManager _customFieldManager;

        public CustomFieldController(IUnitOfWork unitOfWork, IMapper mapper, ICustomFieldManager customFieldManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _customFieldManager = customFieldManager;
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

                    var customFieldHeader = _unitOfWork.CustomFieldHeaders.GetCustomFieldHeader(parsedId, true);

                    if (customFieldHeader != default)
                    {
                        return Ok(_mapper.Map<CustomFieldHead>(customFieldHeader));
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
        public IActionResult Post([FromBody] CustomFieldHead customFieldHead)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (customFieldHead.Id == Guid.Empty)
                    {
                        var customFieldGroupHeader = _unitOfWork.CustomFieldGroupHeaders.GetCustomFieldGroupHeaderWithRelations(customFieldHead.CustomFieldGroupHeaderId, true);

                        if (customFieldGroupHeader != null)
                        {
                            var customFieldGroupHead = _mapper.Map<CustomFieldGroupHead>(customFieldGroupHeader);
                            //if (customFieldGroupHeader.CustomFieldHeaders.All(ch => ch.Name != customFieldHead.Name))
                            //{
                            var sequenceNo = 0; 

                            var maxCustomFieldHead = customFieldGroupHeader.CustomFieldHeaders.OrderByDescending(cf => cf.SequenceNo).FirstOrDefault();

                            if (maxCustomFieldHead != default)
                                sequenceNo = maxCustomFieldHead.SequenceNo + 1;

                            customFieldHead.SequenceNo = sequenceNo;

                            customFieldHead.ShortName = _customFieldManager.SetCustomFieldShortName(customFieldHead, customFieldGroupHead);

                            if(string.IsNullOrWhiteSpace(customFieldHead.ShortName))
                                return StatusCode(StatusCodes.Status400BadRequest,
                                    new APIErrorMessage(StatusCodes.Status400BadRequest,
                                        $"CustomFieldHeader.ShortName: ShortName error, cannot be blank."));

                            if (_customFieldManager.DuplicateFieldShortName(customFieldGroupHead.CustomFieldHeaders, customFieldHead.ShortName))
                                return StatusCode(StatusCodes.Status400BadRequest,
                                    new APIErrorMessage(StatusCodes.Status400BadRequest,
                                        $"CustomFieldHeader.ShortName: '{customFieldHead.ShortName}' Duplicate ShortName"));

                            var customFieldHeader = _mapper.Map<CustomFieldHeader>(customFieldHead);

                            _unitOfWork.CustomFieldHeaders.Add(customFieldHeader);

                            _unitOfWork.Complete();

                            return Created(new Uri($"{Request.Path}/{customFieldHeader.Id}", UriKind.Relative),
                                _mapper.Map<CustomFieldHead>(customFieldHeader));
                            //}

                            //return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"CustomFieldHead.Name: '{customFieldHead.Name}' already exists."));
                        }

                        return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"CustomFieldHead.CustomFieldGroupHeadId: '{customFieldHead.CustomFieldGroupHeaderId}' not found."));
                    }

                    return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"CustomFieldHead.Id: '{customFieldHead.Id}' is not an empty guid."));
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
        public IActionResult Patch(string id, [FromBody] JsonPatchDocument<CustomFieldHead> customFieldHeadPatch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Guid.TryParse(id, out Guid parsedId))
                    {
                        var customFieldHeader = _unitOfWork.CustomFieldHeaders.Get(parsedId);

                        if (customFieldHeader != null)
                        {
                            var customFieldGroupHeader = _unitOfWork.CustomFieldGroupHeaders.GetCustomFieldGroupHeaderWithRelations(customFieldHeader.CustomFieldGroupHeaderId, true);
                            var customFieldHead = _mapper.Map<CustomFieldHead>(customFieldHeader);

                            customFieldHeadPatch.ApplyTo(customFieldHead);

                            if(customFieldHead.CustomFieldType != customFieldHeader.CustomFieldType)
                            {
                                return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"CustomFieldHead.CustomFieldType: '{customFieldHeader.CustomFieldType}' cannot be changed."));
                            }

                            //if(customFieldHead.Name != customFieldHeader.Name)
                            //{
                            //    return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"CustomFieldHead.Name: '{customFieldHeader.Name}' cannot be changed."));
                            //}

                            if(customFieldHead.SequenceNo != customFieldHeader.SequenceNo)
                            {
                                if(customFieldGroupHeader.CustomFieldHeaders.Where(cf => cf.SequenceNo == customFieldHead.SequenceNo).FirstOrDefault() != default)
                                {
                                    return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"CustomFieldHead.SequenceNo: '{customFieldHeader.SequenceNo}' is already in use."));
                                }
                            }

                            if(customFieldHead.ShortName != customFieldHeader.ShortName)
                            {
                                if (string.IsNullOrWhiteSpace(customFieldHead.ShortName))
                                    return StatusCode(StatusCodes.Status400BadRequest,
                                        new APIErrorMessage(StatusCodes.Status400BadRequest,
                                            $"CustomFieldHeader.ShortName: ShortName error, cannot be blank."));

                                if (_customFieldManager.DuplicateFieldShortName(_mapper.Map<CustomFieldGroupHead>(customFieldGroupHeader).CustomFieldHeaders, customFieldHead.ShortName))
                                    return StatusCode(StatusCodes.Status400BadRequest,
                                        new APIErrorMessage(StatusCodes.Status400BadRequest,
                                            $"CustomFieldHeader.ShortName: ShortName is a duplicate."));
                            }

                            _mapper.Map(customFieldHead, customFieldHeader);

                            _unitOfWork.Complete();

                            return Ok(_mapper.Map<CustomFieldHead>(customFieldHeader));
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

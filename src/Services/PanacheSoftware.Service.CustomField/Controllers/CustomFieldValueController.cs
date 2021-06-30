using AutoMapper;
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
    public class CustomFieldValueController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICustomFieldManager _customFieldManager;

        public CustomFieldValueController(IUnitOfWork unitOfWork, IMapper mapper, ICustomFieldManager customFieldManager)
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
                    var customFieldValue = _unitOfWork.CustomFieldValues.Get(parsedId);

                    if (customFieldValue != default)
                    {
                        return Ok(_mapper.Map<CustomFieldVal>(customFieldValue));
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
        public async Task<IActionResult> Post([FromBody] CustomFieldVal customFieldVal)
        {
            if (ModelState.IsValid)
            {              
                try
                {
                    if (customFieldVal.Id == Guid.Empty)
                    {
                        var customFieldHeader = _unitOfWork.CustomFieldHeaders.GetCustomFieldHeader(customFieldVal.CustomFieldHeaderId, true);

                        if (customFieldHeader != null)
                        {
                            var createResult = _customFieldManager.CreateCustomFieldValue(customFieldVal);

                            if(!string.IsNullOrWhiteSpace(createResult.Item2))
                                return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, createResult.Item2));

                            return Created(new Uri($"{Request.Path}/{createResult.Item1.Id}", UriKind.Relative),
                                    createResult.Item1);
                        }

                        return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"CustomFieldVal.CustomFieldHeaderId: '{customFieldVal.CustomFieldHeaderId}' not found."));
                    }

                    return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"CustomFieldVal.Id: '{customFieldVal.Id}' is not an empty guid."));
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
        public IActionResult Patch(string id, [FromBody] JsonPatchDocument<CustomFieldVal> customFieldValPatch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Guid.TryParse(id, out Guid parsedId))
                    {
                        var customFieldValue = _unitOfWork.CustomFieldValues.Get(parsedId);

                        if (customFieldValue != null)
                        {
                            var historicCustomFieldValue = new CustomFieldValue()
                            {
                                Status = customFieldValue.Status,
                                CustomFieldHeaderId = customFieldValue.CustomFieldHeaderId,
                                Id = customFieldValue.Id,
                                FieldValue = customFieldValue.FieldValue,
                                LinkId = customFieldValue.LinkId,
                                LinkType = customFieldValue.LinkType,
                                LastUpdateDate = customFieldValue.LastUpdateDate
                            };

                            var customFieldVal = _mapper.Map<CustomFieldVal>(customFieldValue);

                            customFieldValPatch.ApplyTo(customFieldVal);

                            _mapper.Map(customFieldVal, customFieldValue);

                            _unitOfWork.Complete();

                            _customFieldManager.CheckForAndCreateHistory(historicCustomFieldValue.Id, historicCustomFieldValue.FieldValue, historicCustomFieldValue.LastUpdateDate);

                            return Ok(_mapper.Map<CustomFieldVal>(customFieldValue));
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

        [Route("[action]/{linkType}/{linkId}")]
        [HttpGet]
        public async Task<IActionResult> GetValuesForLink(string linkType, string linkId)
        {
            try
            {
                if (Guid.TryParse(linkId, out Guid parsedId))
                {
                    var customFieldValList = new CustomFieldValList();

                    var customFieldValues = await _unitOfWork.CustomFieldValues.GetCustomFieldValuesForLinkAsync(parsedId, linkType, true);

                    foreach (var customFieldValue in customFieldValues)
                    {
                        customFieldValList.CustomFieldValues.Add(_mapper.Map<CustomFieldVal>(customFieldValue));
                    }

                    return Ok(customFieldValList);
                }

                return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"linkId: '{linkId}' is not a valid guid."));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }
        }
    }
}

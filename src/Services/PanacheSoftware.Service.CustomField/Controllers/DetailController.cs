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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.CustomField.Controllers
{
    [Authorize]
    [Route("CustomFieldGroup/[controller]")]
    [ApiController]
    public class DetailController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DetailController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

                    var customFieldGroupDetail = _unitOfWork.CustomFieldGroupDetails.GetCustomFieldGroupDetail(parsedId, true);

                    if (customFieldGroupDetail != default)
                    {
                        return Ok(_mapper.Map<CustomFieldGroupDetail>(customFieldGroupDetail));
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
        public IActionResult Post([FromBody] CustomFieldGroupDet customFieldGroupDet)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (customFieldGroupDet.Id == Guid.Empty)
                    {
                        var customFieldGroupHeader = _unitOfWork.CustomFieldGroupHeaders.SingleOrDefault(c => c.Id == customFieldGroupDet.CustomFieldGroupHeaderId, true);

                        if (customFieldGroupHeader != default)
                        {
                            var customFieldGroupDetail = _mapper.Map<CustomFieldGroupDetail>(customFieldGroupDet);

                            _unitOfWork.CustomFieldGroupDetails.Add(customFieldGroupDetail);

                            _unitOfWork.Complete();

                            return Created(new Uri($"{Request.Path}/{customFieldGroupDetail.Id}", UriKind.Relative),
                                _mapper.Map<CustomFieldGroupDet>(customFieldGroupDetail));
                        }

                        return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"CustomFieldGroupDet.CustomFieldGroupHeaderId: '{customFieldGroupDet.CustomFieldGroupHeaderId}' not found."));
                    }

                    return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"CustomFieldGroupDet.Id: '{customFieldGroupDet.Id}' is not an empty guid."));
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
        public IActionResult Patch(string id, [FromBody] JsonPatchDocument<CustomFieldGroupDet> customFieldGroupDetPatch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Guid.TryParse(id, out Guid parsedId))
                    {
                        CustomFieldGroupDetail customFieldGroupDetail = _unitOfWork.CustomFieldGroupDetails.Get(parsedId);

                        if (customFieldGroupDetail != null)
                        {

                            var customFieldGroupDet = _mapper.Map<CustomFieldGroupDet>(customFieldGroupDetail);

                            customFieldGroupDetPatch.ApplyTo(customFieldGroupDet);

                            _mapper.Map(customFieldGroupDet, customFieldGroupDetail);

                            _unitOfWork.Complete();

                            return Ok(_mapper.Map<CustomFieldGroupDet>(customFieldGroupDetail));
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

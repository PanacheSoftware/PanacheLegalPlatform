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
    [Route("[controller]")]
    [ApiController]
    public class CustomFieldGroupLinkController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomFieldGroupLinkController(IUnitOfWork unitOfWork, IMapper mapper)
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

                    var customFieldGroupLink = _unitOfWork.CustomFieldGroupLinks.GetCustomFieldGroupLink(parsedId, true);

                    if (customFieldGroupLink != default)
                    {
                        return Ok(_mapper.Map<CustomFieldGroupLnk>(customFieldGroupLink));
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
        public async Task<IActionResult> Post([FromBody] CustomFieldGroupLnk customFieldGroupLnk)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (customFieldGroupLnk.Id == Guid.Empty)
                    {
                        var customFieldGroupHeader = _unitOfWork.CustomFieldGroupHeaders.GetCustomFieldGroupHeader(customFieldGroupLnk.CustomFieldGroupHeaderId, true);

                        if (customFieldGroupHeader != null)
                        {
                            var customFieldGroupLinks = await _unitOfWork.CustomFieldGroupLinks.GetFileLinksWithRelationsForLinkAsync(customFieldGroupLnk.LinkId, customFieldGroupLnk.LinkType, true);

                            if(customFieldGroupLinks.Any())
                            {
                                if(customFieldGroupLinks.Where(l => l.CustomFieldGroupHeaderId == customFieldGroupLnk.CustomFieldGroupHeaderId).Any())
                                    return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"Link already exists."));
                            }

                            var customFieldGroupLink = _mapper.Map<CustomFieldGroupLink>(customFieldGroupLnk);

                            _unitOfWork.CustomFieldGroupLinks.Add(customFieldGroupLink);

                            _unitOfWork.Complete();

                            return Created(new Uri($"{Request.Path}/{customFieldGroupLink.Id}", UriKind.Relative),
                                    _mapper.Map<CustomFieldGroupLnk>(customFieldGroupLink));
                        }

                        return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"CustomFieldGroupLnk.CustomFieldGroupHeadId: '{customFieldGroupLnk.CustomFieldGroupHeaderId}' not found."));
                    }

                    return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"CustomFieldGroupLnk.Id: '{customFieldGroupLnk.Id}' is not an empty guid."));
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
        public IActionResult Patch(string id, [FromBody] JsonPatchDocument<CustomFieldGroupLnk> customFieldGroupLnkPatch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Guid.TryParse(id, out Guid parsedId))
                    {
                        var customFieldGroupLink = _unitOfWork.CustomFieldGroupLinks.Get(parsedId);

                        if (customFieldGroupLink != null)
                        {
                            var customFieldGroupLnk = _mapper.Map<CustomFieldGroupLnk>(customFieldGroupLink);

                            customFieldGroupLnkPatch.ApplyTo(customFieldGroupLnk);

                            if(customFieldGroupLnk.CustomFieldGroupHeaderId == customFieldGroupLink.CustomFieldGroupHeaderId
                                && customFieldGroupLnk.LinkId == customFieldGroupLink.LinkId
                                && customFieldGroupLnk.LinkType == customFieldGroupLink.LinkType)
                            {
                                _mapper.Map(customFieldGroupLnk, customFieldGroupLink);

                                _unitOfWork.Complete();

                                return CreatedAtRoute("Get", new { id = _mapper.Map<CustomFieldGroupLnk>(customFieldGroupLink).Id },
                                _mapper.Map<CustomFieldGroupLnk>(customFieldGroupLink));
                            }

                            return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"Cannot change CustomFieldGroupHeaderId, LinkId or LinkType."));
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
        public async Task<IActionResult> GetLinks(string linkType, string linkId)
        {
            try
            {
                if (Guid.TryParse(linkId, out Guid parsedId))
                {
                    var customFieldGroupLinkList = new CustomFieldGroupLnkList();

                    var customFieldGroupLinks = await _unitOfWork.CustomFieldGroupLinks.GetFileLinksWithRelationsForLinkAsync(parsedId, linkType, true);

                    foreach (var customFieldGroupLink in customFieldGroupLinks)
                    {
                        customFieldGroupLinkList.CustomFieldGroupLinks.Add(_mapper.Map<CustomFieldGroupLnk>(customFieldGroupLink));
                    }

                    return Ok(customFieldGroupLinkList);
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

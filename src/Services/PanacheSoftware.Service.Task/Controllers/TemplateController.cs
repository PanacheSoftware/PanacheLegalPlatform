using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.Error;
using PanacheSoftware.Core.Domain.API.Task.Template;
using PanacheSoftware.Core.Domain.Task.Template;
using PanacheSoftware.Service.Task.Core;
using PanacheSoftware.Service.Task.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITemplateManager _templateManager;
        private readonly IMapper _mapper;

        public TemplateController(IUnitOfWork unitOfWork, ITemplateManager templateManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _templateManager = templateManager;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                var templateHeadList = await _templateManager.GetTemplateHeadListAsync(accessToken);

                if (templateHeadList.TemplateHeaders.Count > 0)
                    return Ok(templateHeadList);

                return NotFound();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
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
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                TemplateHeader templateHeader = null;

                if (Guid.TryParse(id, out Guid parsedId))
                {
                    templateHeader = await _unitOfWork.TemplateHeaders.GetTemplateHeaderWithRelationsAsync(parsedId, false, accessToken);
                }
                else
                {
                    templateHeader = await _unitOfWork.TemplateHeaders.GetTemplateHeaderWithRelationsAsync(id, false, accessToken);
                }

                if (templateHeader != null)
                {
                    return Ok(_mapper.Map<TemplateHead>(templateHeader));
                }

                return NotFound();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TemplateHead templateHead)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");

                    if (templateHead.Id == Guid.Empty)
                    {
                        var templateHeader = _mapper.Map<TemplateHeader>(templateHead);

                        _unitOfWork.TemplateHeaders.Add(templateHeader);

                        _unitOfWork.Complete();

                        return Created(new Uri($"{Request.Path}/{templateHeader.Id}", UriKind.Relative),
                            _mapper.Map<TemplateHead>(templateHeader));
                    }

                    return StatusCode(StatusCodes.Status400BadRequest,
                        new APIErrorMessage(StatusCodes.Status400BadRequest,
                            $"TemplateHead.Id: '{templateHead.Id}' is not an empty guid."));
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
        public async Task<IActionResult> Patch(string id, [FromBody]JsonPatchDocument<TemplateHead> templateHeadPatch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Guid.TryParse(id, out Guid parsedId))
                    {
                        var templateHeader = _unitOfWork.TemplateHeaders.Get(parsedId);

                        if (templateHeader != null)
                        {
                            var templateHead = _mapper.Map<TemplateHead>(templateHeader);

                            templateHeadPatch.ApplyTo(templateHead);

                            _mapper.Map(templateHead, templateHeader);

                            _unitOfWork.Complete();

                            return Ok(_mapper.Map<TemplateHead>(templateHeader));
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

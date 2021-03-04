using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class CustomFieldGroupController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomFieldGroupController(IUnitOfWork unitOfWork, IMapper mapper)
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
    }
}

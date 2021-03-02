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
    public class CustomFieldController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomFieldController(IUnitOfWork unitOfWork, IMapper mapper)
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
        public async Task<IActionResult> Post([FromBody] CustomFieldHead customFieldHead)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");

                    if (customFieldHead.Id == Guid.Empty)
                    {
                        var customFieldHeader = _mapper.Map<CustomFieldHeader>(customFieldHead);

                        _unitOfWork.CustomFieldHeaders.Add(customFieldHeader);

                        _unitOfWork.Complete();

                        return Created(new Uri($"{Request.Path}/{customFieldHeader.Id}", UriKind.Relative),
                            _mapper.Map<CustomFieldHead>(customFieldHeader));
                    }

                    return StatusCode(StatusCodes.Status400BadRequest,
                        new APIErrorMessage(StatusCodes.Status400BadRequest,
                            $"CustomFieldHead.Id: '{customFieldHead.Id}' is not an empty guid."));
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

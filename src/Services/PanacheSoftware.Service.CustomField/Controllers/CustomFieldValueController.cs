using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.CustomField;
using PanacheSoftware.Core.Domain.API.Error;
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
    public class CustomFieldValueController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomFieldValueController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.Automation;
using PanacheSoftware.Core.Domain.API.Error;
using PanacheSoftware.Service.Automation.Manager;
using System;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Automation.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentManager _documentManager;

        public DocumentController(IDocumentManager documentManager)
        {
            _documentManager = documentManager;
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


                    var autoDoc = new AutoDoc();

                    autoDoc.FileHeaderId = parsedId;
                    autoDoc.AutoValues.Add(new AutoVal("TEST-FIELD-1", "Test Field 1 Value"));
                    autoDoc.AutoValues.Add(new AutoVal("TEST-FIELD-2", "Test Field 2 Value"));
                    autoDoc.AutoValues.Add(new AutoVal("TEST-FIELD-3", "Test Field 3 Value"));
                    autoDoc.AutoValues.Add(new AutoVal("TEST-FIELD-4", "Test Field 4 Value"));

                    await _documentManager.AutomateDocument(autoDoc, accessToken);

                    return Ok(autoDoc);

                    //var customFieldHeader = _unitOfWork.CustomFieldHeaders.GetCustomFieldHeader(parsedId, true);

                    //if (customFieldHeader != default)
                    //{
                    //    return Ok(_mapper.Map<CustomFieldHead>(customFieldHeader));
                    //}
                }

                return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"id: '{id}' is not a valid guid."));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }
        }

        [Route("[action]/{fileId}/{linkId}/{linkType}")]
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AutomateFromLink(string fileId, string linkId, string linkType)
        {
            try
            {
                if (Guid.TryParse(fileId, out Guid parsedId) && Guid.TryParse(linkId, out Guid parsedLinkId))
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");

                    var autoDoc = new AutoDoc();
                    autoDoc.FileHeaderId = parsedId;
                    
                    await _documentManager.AutomateDocumentFromLink(autoDoc, accessToken, parsedLinkId, linkType);

                    return Ok(autoDoc);
                }

                return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"fileId: '{fileId}' is not a valid guid."));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }
        }
    }
}

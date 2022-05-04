using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.Error;
using PanacheSoftware.Core.Domain.API.File;
using PanacheSoftware.Core.Domain.File;
using PanacheSoftware.Service.File.Core.Repositories;
using PanacheSoftware.Service.File.Manager;

namespace PanacheSoftware.Service.File.Controllers
{
    [Authorize]
    [Route("File/Link")]
    [ApiController]
    public class FileLinkController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileManager _fileManager;

        public FileLinkController(IUnitOfWork unitOfWork, IMapper mapper, IFileManager fileManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileManager = fileManager;
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
                    var fileLink = await _unitOfWork.FileLinks.GetLinkWithRelationsAsync(parsedId, true);

                    if (fileLink != null)
                        return Ok(_mapper.Map<FileLnk>(fileLink));

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

        [HttpPost]
        public IActionResult Post([FromBody]FileLnk fileLnk)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (fileLnk.Id == Guid.Empty)
                    {
                        var fileLink = _mapper.Map<FileLink>(fileLnk);

                        _unitOfWork.FileLinks.Add(fileLink);

                        _unitOfWork.Complete();

                        return Created(new Uri($"{Request.Path}/{fileLink.Id}", UriKind.Relative),
                            _mapper.Map<FileLnk>(fileLink));
                    }

                    return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"FileLnk.Id: '{fileLnk.Id}' is not an empty guid."));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<FileLnk> fileLinkPatch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Guid.TryParse(id, out Guid parsedId))
                    {
                        var fileLink = _unitOfWork.FileLinks.Get(parsedId);

                        if (fileLink != null)
                        {
                            var fileLnk = _mapper.Map<FileLnk>(fileLink);

                            fileLinkPatch.ApplyTo(fileLnk);

                            _mapper.Map(fileLnk, fileLink);

                            _unitOfWork.Complete();

                            return Ok();
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
        public async Task<IActionResult> GetFilesForLink(string linkType, string linkId)
        {
            try
            {
                if (Guid.TryParse(linkId, out Guid parsedId))
                {
                    var fileList = await _fileManager.GetFileListForLinkAsync(parsedId, linkType);

                    return Ok(fileList);
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
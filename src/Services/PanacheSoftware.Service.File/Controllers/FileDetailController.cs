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

namespace PanacheSoftware.Service.File.Controllers
{
    [Authorize]
    [Route("File/Detail")]
    [ApiController]
    public class FileDetailController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FileDetailController(IUnitOfWork unitOfWork, IMapper mapper)
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
                    FileDetail fileDetail = null;

                    //Treat 'id' as a fileHeaderId and if not found as a fileDetailId
                    fileDetail = await _unitOfWork.FileDetails.GetFileDetailWithRelationsAsync(parsedId, true) 
                                 ?? await _unitOfWork.FileDetails.GetDetailWithRelationsAsync(parsedId, true);

                    if (fileDetail != null)
                        return Ok(_mapper.Map<FileDet>(fileDetail));

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
        public IActionResult Post([FromBody]FileDet fileDet)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (fileDet.Id == Guid.Empty)
                    {
                        FileHeader fileHeader =
                            _unitOfWork.FileHeaders.SingleOrDefault(c => c.Id == fileDet.FileHeaderId, true);

                        if (fileHeader != null)
                        {
                            var fileDetail = _mapper.Map<FileDetail>(fileDet);

                            _unitOfWork.FileDetails.Add(fileDetail);

                            _unitOfWork.Complete();

                            return Created(new Uri($"{Request.Path}/{fileDetail.Id}", UriKind.Relative),
                                _mapper.Map<FileDet>(fileDetail));
                        }

                        return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"FileDet.FileHeaderId: '{fileDet.FileHeaderId}' not found."));
                    }

                    return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"FileDet.Id: '{fileDet.Id}' is not an empty guid."));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<FileDet> fileDetPatch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Guid.TryParse(id, out Guid parsedId))
                    {
                        var fileDetail = _unitOfWork.FileDetails.Get(parsedId);

                        if (fileDetail != null)
                        {

                            var fileDet = _mapper.Map<FileDet>(fileDetail);

                            fileDetPatch.ApplyTo(fileDet);

                            _mapper.Map(fileDet, fileDetail);

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
    }
}
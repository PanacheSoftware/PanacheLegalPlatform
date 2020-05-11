using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.File;
using PanacheSoftware.Core.Domain.File;
using PanacheSoftware.Service.File.Core.Repositories;

namespace PanacheSoftware.Service.File.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
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

                    fileDetail = await _unitOfWork.FileDetails.GetFileDetailWithRelationsAsync(parsedId, true);

                    if (fileDetail == null)
                    {
                        fileDetail = await _unitOfWork.FileDetails.GetDetailWithRelationsAsync(parsedId, true);
                    }

                    if (fileDetail != null)
                        return Ok(_mapper.Map<FileDet>(fileDetail));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Post([FromBody]FileDet fileDet)
        {
            try
            {
                if (fileDet.Id == Guid.Empty)
                {
                    FileHeader fileHeader = _unitOfWork.FileHeaders.SingleOrDefault(c => c.Id == fileDet.FileHeaderId, true);

                    if (fileHeader != null)
                    {
                        var fileDetail = _mapper.Map<FileDetail>(fileDet);

                        _unitOfWork.FileDetails.Add(fileDetail);

                        _unitOfWork.Complete();

                        return Created(new Uri($"{Request.Path}/{fileDetail.Id}", UriKind.Relative), _mapper.Map<FileDet>(fileDetail));
                    }
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return BadRequest();
        }

        [HttpPatch("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<FileDet> fileDetPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    var fileDetail = _unitOfWork.FileDetails.Get(parsedId);

                    var fileDet = _mapper.Map<FileDet>(fileDetail);

                    fileDetPatch.ApplyTo(fileDet);

                    _mapper.Map(fileDet, fileDetail);

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Get", new { id = _mapper.Map<FileDet>(fileDetail).Id }, _mapper.Map<FileDet>(fileDetail));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return BadRequest();
        }
    }
}
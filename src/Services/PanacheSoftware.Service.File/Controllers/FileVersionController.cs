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
using PanacheSoftware.Service.File.Manager;

namespace PanacheSoftware.Service.File.Controllers
{
    [Authorize]
    [Route("api/File/Version")]
    [ApiController]
    public class FileVersionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileManager _fileManager;

        public FileVersionController(IUnitOfWork unitOfWork, IMapper mapper, IFileManager fileManager)
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
        public async Task<IActionResult> GetAsync(string id)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    var fileVersion = await _unitOfWork.FileVersions.GetVersionWithRelationsAsync(parsedId, true);

                    if (fileVersion != null)
                        return Ok(_mapper.Map<FileVer>(fileVersion));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody]FileVer fileVer)
        {
            try
            {
                if (fileVer.Id == Guid.Empty)
                {
                    var fileVersion = _mapper.Map<FileVersion>(fileVer);

                    await _fileManager.SetFileVersionSequenceNoAsync(fileVersion);

                    _unitOfWork.FileVersions.Add(fileVersion);

                    _unitOfWork.Complete();

                    return Created(new Uri($"{Request.Path}/{fileVersion.Id}", UriKind.Relative), _mapper.Map<FileVer>(fileVersion));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<FileVer> fileVerPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    var fileVersion = _unitOfWork.FileVersions.Get(parsedId);

                    var fileVer = _mapper.Map<FileVer>(fileVersion);

                    fileVerPatch.ApplyTo(fileVer);

                    _mapper.Map(fileVer, fileVersion);

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Get", new { id = _mapper.Map<FileVer>(fileVersion).Id }, _mapper.Map<FileVer>(fileVersion));
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
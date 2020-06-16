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
    [Route("[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileManager _fileManager;

        public FileController(IUnitOfWork unitOfWork, IMapper mapper, IFileManager fileManager)
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
                    var fileHeader = await _unitOfWork.FileHeaders.GetFileHeaderWithRelationsAsync(parsedId, true);

                    if(fileHeader != null)
                        return Ok(_mapper.Map<FileHead>(fileHeader));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get()
        {
            var fileList = new FileList();

            try
            {
                var fileHeaders = await _unitOfWork.FileHeaders.GetFileHeadersWithRelationsAsync(true);

                if(fileHeaders.Any())
                {
                    foreach (var fileHeader in fileHeaders)
                    {
                        fileList.FileHeaders.Add(_mapper.Map<FileHead>(fileHeader));
                    }
                }    
            }
            catch (Exception e)
            {
                string message = e.Message;
                return NotFound();
            }

            return Ok(fileList);
        }

        [HttpPost]
        public IActionResult Post([FromBody]FileHead fileHead)
        {
            try
            {
                if (fileHead.Id == Guid.Empty)
                {
                    var fileHeader = _mapper.Map<FileHeader>(fileHead);

                    _unitOfWork.FileHeaders.Add(fileHeader);

                    _unitOfWork.Complete();

                    return Created(new Uri($"{Request.Path}/{fileHeader.Id}", UriKind.Relative), _mapper.Map<FileHead>(fileHeader));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<FileHead> fileHeadPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    var fileHeader = _unitOfWork.FileHeaders.Get(parsedId);

                    var fileHead = _mapper.Map<FileHead>(fileHeader);

                    fileHeadPatch.ApplyTo(fileHead);

                    _mapper.Map(fileHead, fileHeader);

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Get", new { id = _mapper.Map<FileHead>(fileHeader).Id }, _mapper.Map<FileHead>(fileHeader));
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
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
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Post([FromBody]FileLnk fileLnk)
        {
            try
            {
                if (fileLnk.Id == Guid.Empty)
                {
                    var fileLink = _mapper.Map<FileLink>(fileLnk);

                    _unitOfWork.FileLinks.Add(fileLink);

                    _unitOfWork.Complete();

                    return Created(new Uri($"{Request.Path}/{fileLink.Id}", UriKind.Relative), _mapper.Map<FileLnk>(fileLink));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<FileLnk> fileLinkPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    var fileLink = _unitOfWork.FileLinks.Get(parsedId);

                    var fileLnk = _mapper.Map<FileLnk>(fileLink);

                    fileLinkPatch.ApplyTo(fileLnk);

                    _mapper.Map(fileLnk, fileLink);

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Get", new { id = _mapper.Map<FileLnk>(fileLink).Id }, _mapper.Map<FileLnk>(fileLink));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return BadRequest();
        }

        [Route("[action]/{linkType}/{linkId}")]
        [HttpGet]
        public async Task<IActionResult> GetFilesForLink(string linkType, string linkId)
        {
            try
            {
                if (Guid.TryParse(linkId, out Guid parsedId))
                {
                    //FileLnkList fileLinks = new FileLnkList();

                    //var links = await _unitOfWork.FileLinks.GetFileLinksWithRelationsForLinkAsync(parsedId, linkType, true);

                    //if (links.Any())
                    //    _mapper.Map(links, fileLinks.FileLinks);

                    //return Ok(fileLinks);

                    var fileList = await _fileManager.GetFileListForLinkAsync(parsedId, linkType);

                    return Ok(fileList);
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }
    }
}
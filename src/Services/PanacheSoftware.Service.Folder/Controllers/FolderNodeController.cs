using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.Folder;
using PanacheSoftware.Core.Domain.Folder;
using PanacheSoftware.Service.Folder.Core;
using PanacheSoftware.Service.Folder.Manager;

namespace PanacheSoftware.Service.Folder.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FolderNodeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFolderManager _folderManager;

        public FolderNodeController(IUnitOfWork unitOfWork, IMapper mapper, IFolderManager folderManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _folderManager = folderManager;
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public IActionResult Get(string id)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    FolderNode folderNode = _unitOfWork.FolderNodes.GetNode(parsedId, true);

                    return Ok(_mapper.Map<FolderNod>(folderNode));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Post([FromBody]FolderNod folderNod)
        {
            try
            {
                if (folderNod.Id == Guid.Empty)
                {
                    FolderHeader folderHeader = _unitOfWork.FolderHeaders.SingleOrDefault(c => c.Id == folderNod.FolderHeaderId, true);

                    if (folderHeader.Id != Guid.Empty)
                    {
                        //var userId = User.FindFirstValue("sub");

                        var folderNode = _mapper.Map<FolderNode>(folderNod);

                        if (!_folderManager.SetNewFolderNodeSequenceNo(folderNode))
                            return BadRequest();

                        _unitOfWork.FolderNodes.Add(folderNode);

                        _unitOfWork.Complete();

                        return Created(new Uri($"{Request.Path}/{folderNode.Id}", UriKind.Relative), _mapper.Map<FolderNod>(folderNode));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<FolderNod> folderNodPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    FolderNode folderNode = _unitOfWork.FolderNodes.Get(parsedId);

                    FolderNod folderNod = _mapper.Map<FolderNod>(folderNode);

                    folderNodPatch.ApplyTo(folderNod);

                    _mapper.Map(folderNod, folderNode);

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Get", new { id = _mapper.Map<FolderNod>(folderNode).Id }, _mapper.Map<FolderNod>(folderNode));
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
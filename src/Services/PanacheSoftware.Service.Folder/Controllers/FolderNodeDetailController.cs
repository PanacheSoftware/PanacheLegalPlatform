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
    public class FolderNodeDetailController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FolderNodeDetailController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
                    FolderNodeDetail folderNodeDetail = _unitOfWork.FolderNodeDetails.GetNodeDetail(parsedId, true);

                    return Ok(_mapper.Map<FolderNodDet>(folderNodeDetail));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Post([FromBody]FolderNodDet folderNodDet)
        {
            try
            {
                if (folderNodDet.Id == Guid.Empty)
                {
                    FolderNode folderNode = _unitOfWork.FolderNodes.SingleOrDefault(c => c.Id == folderNodDet.FolderNodeId, true);

                    if (folderNode.Id != Guid.Empty)
                    {
                        //var userId = User.FindFirstValue("sub");

                        var folderNodeDetail = _mapper.Map<FolderNodeDetail>(folderNodDet);

                        _unitOfWork.FolderNodeDetails.Add(folderNodeDetail);

                        _unitOfWork.Complete();

                        return Created(new Uri($"{Request.Path}/{folderNodeDetail.Id}", UriKind.Relative), _mapper.Map<FolderNodDet>(folderNodeDetail));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<FolderNodDet> folderNodDetPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    //var userId = User.FindFirstValue("sub");

                    FolderNodeDetail folderNodeDetail = _unitOfWork.FolderNodeDetails.Get(parsedId);

                    FolderNodDet folderNodDet = _mapper.Map<FolderNodDet>(folderNodeDetail);

                    folderNodDetPatch.ApplyTo(folderNodDet);

                    _mapper.Map(folderNodDet, folderNodeDetail);

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Get", new { id = _mapper.Map<FolderNodDet>(folderNodeDetail).Id }, _mapper.Map<FolderNodDet>(folderNodeDetail));
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
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

namespace PanacheSoftware.Service.Folder.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FolderDetailController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FolderDetailController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //GET: api/Detail/5
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
                    FolderDetail folderDetail = _unitOfWork.FolderDetails.GetDetail(parsedId, true);

                    return Ok(_mapper.Map<FolderDet>(folderDetail));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Post([FromBody]FolderDet folderDet)
        {
            try
            {
                if (folderDet.Id == Guid.Empty)
                {
                    FolderHeader folderHeader = _unitOfWork.FolderHeaders.SingleOrDefault(c => c.Id == folderDet.FolderHeaderId, true);

                    if (folderHeader.Id != Guid.Empty)
                    {
                        //var userId = User.FindFirstValue("sub");

                        var folderDetail = _mapper.Map<FolderDetail>(folderDet);

                        _unitOfWork.FolderDetails.Add(folderDetail);

                        _unitOfWork.Complete();

                        return Created(new Uri($"{Request.Path}/{folderDetail.Id}", UriKind.Relative), _mapper.Map<FolderDet>(folderDetail));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<FolderDet> folderDetPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    //var userId = User.FindFirstValue("sub");

                    FolderDetail folderDetail = _unitOfWork.FolderDetails.Get(parsedId);

                    FolderDet folderDet = _mapper.Map<FolderDet>(folderDetail);

                    folderDetPatch.ApplyTo(folderDet);

                    _mapper.Map(folderDet, folderDetail);

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Get", new { id = _mapper.Map<FolderDet>(folderDetail).Id }, _mapper.Map<FolderDet>(folderDetail));
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
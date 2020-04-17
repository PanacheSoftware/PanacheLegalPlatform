using System;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.Folder;
using PanacheSoftware.Core.Domain.Folder;
using PanacheSoftware.Service.Folder.Core;
using PanacheSoftware.Service.Folder.Manager;

namespace PanacheSoftware.Service.Folder.Controllers
{
    [Produces("application/json")]
    [Authorize]
    [Route("api/[controller]")]
    public class FolderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFolderManager _folderManager;

        public FolderController(IUnitOfWork unitOfWork, IMapper mapper, IFolderManager folderManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _folderManager = folderManager;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public IActionResult Get()
        {
            try
            {
                FolderList folderList = _folderManager.GetFolderList();

                if (folderList.FolderHeaders.Count > 0)
                    return Ok(folderList);
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
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
                FolderHeader folderHeader = null;

                if (Guid.TryParse(id, out Guid parsedId))
                {
                    folderHeader = _unitOfWork.FolderHeaders.GetFolderHeaderWithRelations(parsedId, false);
                }
                else
                {
                    folderHeader = _unitOfWork.FolderHeaders.GetFolderHeaderWithRelations(id, false);
                }

                if (folderHeader != null)
                {
                    return Ok(_mapper.Map<FolderHead>(folderHeader));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Post([FromBody]FolderHead folderHead)
        {
            try
            {
                if (folderHead.Id == Guid.Empty)
                {
                    //var userId = User.FindFirstValue("sub");

                    var folderHeader = _mapper.Map<FolderHeader>(folderHead);

                    if (!_folderManager.FolderParentOkay(folderHeader))
                        return BadRequest();

                    if (!_folderManager.SetNewFolderSequenceNo(folderHeader))
                        return BadRequest();

                    folderHeader.OriginalCompletionDate = folderHeader.CompletionDate;
                    folderHeader.CompletedOnDate = DateTime.Parse("01/01/1900");
                    folderHeader.Completed = false;

                    _unitOfWork.FolderHeaders.Add(folderHeader);

                    _unitOfWork.Complete();

                    return Created(new Uri($"{Request.Path}/{folderHeader.Id}", UriKind.Relative), _mapper.Map<FolderHead>(folderHeader));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<FolderHead> folderHeadPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    //var userId = User.FindFirstValue("sub");

                    FolderHeader folderHeader = _unitOfWork.FolderHeaders.Get(parsedId);

                    FolderHead folderHead = _mapper.Map<FolderHead>(folderHeader);

                    folderHeadPatch.ApplyTo(folderHead);

                    _mapper.Map(folderHead, folderHeader);

                    if (!_folderManager.FolderParentOkay(folderHeader))
                        return BadRequest();

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Get", new { id = _mapper.Map<FolderHead>(folderHeader).Id }, _mapper.Map<FolderHead>(folderHeader));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return BadRequest();
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult GetMainFolders()
        {
            try
            {
                FolderList folderList = _folderManager.GetMainFolders();

                if (folderList.FolderHeaders.Count > 0)
                    return Ok(folderList);
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public IActionResult GetValidParents(string id)
        {
            FolderList folderList;

            if (Guid.TryParse(id, out Guid parsedId))
            {
                folderList = _folderManager.GetFolderList(parsedId, true);

                if (folderList.FolderHeaders.Count > 0)
                    return Ok(folderList);

                return NotFound();
            }

            return BadRequest();
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public IActionResult GetFolderStructure(string id)
        {
            if (Guid.TryParse(id, out Guid parsedId))
            {
                FolderStruct folderStruct = _folderManager.GetFolderStructure(parsedId);

                if (folderStruct.Id != Guid.Empty)
                    return Ok(folderStruct);

                return NotFound();
            }

            return BadRequest();
        }
    }
}
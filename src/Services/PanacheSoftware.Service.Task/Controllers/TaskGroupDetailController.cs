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
using PanacheSoftware.Core.Domain.API.Task;
using PanacheSoftware.Core.Domain.Task;
using PanacheSoftware.Service.Task.Core;

namespace PanacheSoftware.Service.Task.Controllers
{
    [Authorize]
    [Route("api/TaskGroup/Detail")]
    [ApiController]
    public class TaskGroupDetailController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TaskGroupDetailController(IUnitOfWork unitOfWork, IMapper mapper)
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
                    var taskGroupDetail = _unitOfWork.TaskGroupDetails.GetDetail(parsedId, true);

                    return Ok(_mapper.Map<TaskGroupDet>(taskGroupDetail));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Post([FromBody]TaskGroupDet taskGroupDet)
        {
            try
            {
                if (taskGroupDet.Id == Guid.Empty)
                {
                    var taskGroupHeader = _unitOfWork.TaskGroupHeaders.SingleOrDefault(c => c.Id == taskGroupDet.TaskGroupHeaderId, true);

                    if (taskGroupHeader.Id != Guid.Empty)
                    {
                        //var userId = User.FindFirstValue("sub");

                        var taskGroupDetail = _mapper.Map<TaskGroupDetail>(taskGroupDet);

                        _unitOfWork.TaskGroupDetails.Add(taskGroupDetail);

                        _unitOfWork.Complete();

                        return Created(new Uri($"{Request.Path}/{taskGroupDetail.Id}", UriKind.Relative), _mapper.Map<TaskGroupDet>(taskGroupDetail));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<TaskGroupDet> taskGroupDetPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    //var userId = User.FindFirstValue("sub");

                    var taskGroupDetail = _unitOfWork.TaskGroupDetails.Get(parsedId);

                    var taskGroupDet = _mapper.Map<TaskGroupDet>(taskGroupDetail);

                    taskGroupDetPatch.ApplyTo(taskGroupDet);

                    _mapper.Map(taskGroupDet, taskGroupDetail);

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Get", new { id = _mapper.Map<TaskGroupDet>(taskGroupDetail).Id }, _mapper.Map<TaskGroupDet>(taskGroupDetail));
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
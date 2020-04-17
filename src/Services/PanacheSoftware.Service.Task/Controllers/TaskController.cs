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
using PanacheSoftware.Service.Task.Manager;

namespace PanacheSoftware.Service.Task.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITaskManager _taskManager;

        public TaskController(IUnitOfWork unitOfWork, IMapper mapper, ITaskManager taskManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _taskManager = taskManager;
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
                    var taskHeader = _unitOfWork.TaskHeaders.GetTaskHeader(parsedId, true);

                    return Ok(_mapper.Map<TaskHead>(taskHeader));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Post([FromBody]TaskHead taskHead)
        {
            try
            {
                if (taskHead.Id == Guid.Empty)
                {
                    TaskGroupHeader taskGroupHeader = _unitOfWork.TaskGroupHeaders.SingleOrDefault(c => c.Id == taskHead.TaskGroupHeaderId, true);

                    if (taskGroupHeader.Id != Guid.Empty)
                    {
                        //var userId = User.FindFirstValue("sub");

                        var taskHeader = _mapper.Map<TaskHeader>(taskHead);

                        if (!_taskManager.SetNewTaskSequenceNo(taskHeader))
                            return BadRequest();

                        _unitOfWork.TaskHeaders.Add(taskHeader);

                        _unitOfWork.Complete();

                        return Created(new Uri($"{Request.Path}/{taskHeader.Id}", UriKind.Relative), _mapper.Map<TaskHead>(taskHeader));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<TaskHead> taskHeadPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    var taskHeader = _unitOfWork.TaskHeaders.Get(parsedId);

                    var taskHead = _mapper.Map<TaskHead>(taskHeader);

                    taskHeadPatch.ApplyTo(taskHead);

                    _mapper.Map(taskHead, taskHeader);

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Get", new { id = _mapper.Map<TaskHead>(taskHeader).Id }, _mapper.Map<TaskHead>(taskHeader));
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
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
    [Route("[controller]")]
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
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");

                    var taskHeader = _unitOfWork.TaskHeaders.GetTaskHeader(parsedId, true);

                    if (await _taskManager.CanAccessTaskGroupHeaderAsync(taskHeader.TaskGroupHeaderId, accessToken))
                    {
                        return Ok(_mapper.Map<TaskHead>(taskHeader));
                    }
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TaskHead taskHead)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                if (taskHead.Id == Guid.Empty)
                {
                    TaskGroupHeader taskGroupHeader = _unitOfWork.TaskGroupHeaders.SingleOrDefault(c => c.Id == taskHead.TaskGroupHeaderId, true);
                    if (taskGroupHeader != null)
                    {
                        if (await _taskManager.CanAccessTaskGroupHeaderAsync(taskGroupHeader.Id, accessToken))
                        {
                            if (taskGroupHeader.Id != Guid.Empty)
                            {
                                var taskHeader = _mapper.Map<TaskHeader>(taskHead);

                                //Make sure the start and completion dates don't fall outside of the group headers dates
                                taskHeader.StartDate = (taskHeader.StartDate < taskGroupHeader.StartDate) ? taskGroupHeader.StartDate : taskHeader.StartDate;
                                taskHeader.CompletionDate = (taskHeader.CompletionDate > taskGroupHeader.CompletionDate) ? taskGroupHeader.CompletionDate : taskHeader.CompletionDate;

                                if (!await _taskManager.SetNewTaskSequenceNoAsync(taskHeader, accessToken))
                                    return BadRequest();

                                _unitOfWork.TaskHeaders.Add(taskHeader);

                                _unitOfWork.Complete();

                                return Created(new Uri($"{Request.Path}/{taskHeader.Id}", UriKind.Relative), _mapper.Map<TaskHead>(taskHeader));
                            }
                        }
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
        public async Task<IActionResult> Patch(string id, [FromBody]JsonPatchDocument<TaskHead> taskHeadPatch)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                if (Guid.TryParse(id, out Guid parsedId))
                {
                    var taskHeader = _unitOfWork.TaskHeaders.Get(parsedId);

                    var taskHead = _mapper.Map<TaskHead>(taskHeader);

                    TaskGroupHeader taskGroupHeader = _unitOfWork.TaskGroupHeaders.SingleOrDefault(c => c.Id == taskHead.TaskGroupHeaderId, true);

                    if (taskGroupHeader != null)
                    {
                        if (await _taskManager.CanAccessTaskGroupHeaderAsync(taskGroupHeader.Id, accessToken))
                        {
                            taskHeadPatch.ApplyTo(taskHead);

                            //Make sure the Original dates do not get changed
                            taskHead.OriginalCompletionDate = taskHeader.OriginalCompletionDate;
                            taskHead.OriginalStartDate = taskHeader.OriginalStartDate;

                            //Make sure the start and completion dates don't fall outside of the group headers dates
                            taskHead.StartDate = (taskHeader.StartDate < taskGroupHeader.StartDate) ? taskGroupHeader.StartDate : taskHeader.StartDate;
                            taskHead.CompletionDate = (taskHeader.CompletionDate > taskGroupHeader.CompletionDate) ? taskGroupHeader.CompletionDate : taskHeader.CompletionDate;

                            _mapper.Map(taskHead, taskHeader);

                            _unitOfWork.Complete();

                            return CreatedAtRoute("Get", new { id = _mapper.Map<TaskHead>(taskHeader).Id }, _mapper.Map<TaskHead>(taskHeader));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return BadRequest();
        }

        [Route("[action]/{id}")]
        [HttpPost]
        public async Task<IActionResult> Complete(string id)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                if (Guid.TryParse(id, out Guid parsedId))
                {
                    var taskHeader = _unitOfWork.TaskHeaders.Get(parsedId);

                    if (taskHeader != null)
                    {
                        if (await _taskManager.CanAccessTaskGroupHeaderAsync(taskHeader.TaskGroupHeaderId, accessToken))
                        {
                            if (!taskHeader.Completed)
                            {
                                taskHeader.Completed = true;

                                if (DateTime.Today <= taskHeader.StartDate)
                                {
                                    taskHeader.CompletedOnDate = taskHeader.CompletionDate;
                                }
                                else
                                {
                                    taskHeader.CompletedOnDate = DateTime.Today;
                                }

                                _unitOfWork.Complete();

                                return Ok(_mapper.Map<TaskHead>(taskHeader));
                            }
                        }
                    }
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
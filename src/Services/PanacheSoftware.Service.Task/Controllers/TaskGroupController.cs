using System;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.Task;
using PanacheSoftware.Core.Domain.Task;
using PanacheSoftware.Service.Task.Core;
using PanacheSoftware.Service.Task.Manager;

namespace PanacheSoftware.Service.Task.Controllers
{
    [Produces("application/json")]
    [Authorize]
    [Route("api/[controller]")]
    public class TaskGroupController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITaskManager _taskManager;

        public TaskGroupController(IUnitOfWork unitOfWork, IMapper mapper, ITaskManager taskManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _taskManager = taskManager;
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
                var taskGroupList = _taskManager.GetTaskGroupList();

                if (taskGroupList.TaskGroupHeaders.Count > 0)
                    return Ok(taskGroupList);
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
                TaskGroupHeader taskGroupHeader = null;

                if (Guid.TryParse(id, out Guid parsedId))
                {
                    taskGroupHeader = _unitOfWork.TaskGroupHeaders.GetTaskGroupHeaderWithRelations(parsedId, false);
                }
                else
                {
                    taskGroupHeader = _unitOfWork.TaskGroupHeaders.GetTaskGroupHeaderWithRelations(id, false);
                }

                if (taskGroupHeader != null)
                {
                    return Ok(_mapper.Map<TaskGroupHead>(taskGroupHeader));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Post([FromBody]TaskGroupHead taskGroupHead)
        {
            try
            {
                if (taskGroupHead.Id == Guid.Empty)
                {
                    //var userId = User.FindFirstValue("sub");

                    var taskGroupHeader = _mapper.Map<TaskGroupHeader>(taskGroupHead);

                    if (!_taskManager.TaskGroupParentOkay(taskGroupHeader))
                        return BadRequest();

                    if (!_taskManager.SetNewTaskGroupSequenceNo(taskGroupHeader))
                        return BadRequest();

                    taskGroupHeader.OriginalCompletionDate = taskGroupHeader.CompletionDate;
                    taskGroupHeader.CompletedOnDate = DateTime.Parse("01/01/1900");
                    taskGroupHeader.Completed = false;

                    _unitOfWork.TaskGroupHeaders.Add(taskGroupHeader);

                    _unitOfWork.Complete();

                    return Created(new Uri($"{Request.Path}/{taskGroupHeader.Id}", UriKind.Relative), _mapper.Map<TaskGroupHead>(taskGroupHeader));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<TaskGroupHead> taskGroupHeadPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    //var userId = User.FindFirstValue("sub");

                    var taskGroupHeader = _unitOfWork.TaskGroupHeaders.Get(parsedId);

                    var taskGroupHead = _mapper.Map<TaskGroupHead>(taskGroupHeader);
                  
                    taskGroupHeadPatch.ApplyTo(taskGroupHead);

                    //Make sure the Original dates do not get changed
                    taskGroupHead.OriginalCompletionDate = taskGroupHeader.OriginalCompletionDate;
                    taskGroupHead.OriginalStartDate = taskGroupHeader.OriginalStartDate;

                    if (taskGroupHeader.ParentTaskGroupId != null)
                    {
                        TaskGroupHeader parentTaskGroupHeader = _unitOfWork.TaskGroupHeaders.SingleOrDefault(c => c.Id == taskGroupHeader.ParentTaskGroupId, true);

                        if(parentTaskGroupHeader != null)
                        {
                            //Make sure the start and completion dates don't fall outside of the group headers dates
                            taskGroupHead.StartDate = (taskGroupHead.StartDate < parentTaskGroupHeader.StartDate) ? parentTaskGroupHeader.StartDate : taskGroupHead.StartDate;
                            taskGroupHead.CompletionDate = (taskGroupHead.CompletionDate > parentTaskGroupHeader.CompletionDate) ? parentTaskGroupHeader.CompletionDate : taskGroupHead.CompletionDate;
                        }
                    }

                    _mapper.Map(taskGroupHead, taskGroupHeader);

                    if (!_taskManager.TaskGroupParentOkay(taskGroupHeader))
                        return BadRequest();

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Get", new { id = _mapper.Map<TaskGroupHead>(taskGroupHeader).Id }, _mapper.Map<TaskGroupHead>(taskGroupHeader));
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
        public IActionResult Complete(string id)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    var taskGroupHeaderReadOnly = _unitOfWork.TaskGroupHeaders.GetTaskGroupHeaderWithRelations(parsedId, true);

                    if(taskGroupHeaderReadOnly != null)
                    {
                        if (!taskGroupHeaderReadOnly.Completed)
                        {
                            if (_taskManager.CanCompleteTaskGroup(taskGroupHeaderReadOnly.Id))
                            {
                                var taskGroupHeader = _unitOfWork.TaskGroupHeaders.Get(taskGroupHeaderReadOnly.Id);

                                if (taskGroupHeader != null)
                                {
                                    taskGroupHeader.Completed = true;

                                    if (DateTime.Today <= taskGroupHeader.StartDate)
                                    {
                                        taskGroupHeader.CompletedOnDate = taskGroupHeader.CompletionDate;
                                    }
                                    else
                                    {
                                        taskGroupHeader.CompletedOnDate = DateTime.Today;
                                    }

                                    _unitOfWork.Complete();

                                    return Ok(_mapper.Map<TaskGroupHead>(taskGroupHeader));
                                }
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

        [Route("[action]")]
        [HttpGet]
        public IActionResult GetMainTaskGroups()
        {
            try
            {
                var taskGroupList = _taskManager.GetMainTaskGroups();

                if (taskGroupList.TaskGroupHeaders.Count > 0)
                    return Ok(taskGroupList);
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult GetTaskGroupSummary()
        {
            try
            {
                var taskGroupList = _taskManager.GetMainTaskGroupSummarys();

                if (taskGroupList.TaskGroupSummarys.Count > 0)
                    return Ok(taskGroupList);
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public IActionResult GetTaskGroupSummary(string id)
        {
            if (Guid.TryParse(id, out Guid parsedId))
            {
                var taskGroupSummary = _taskManager.GetTaskGroupSummary(parsedId);

                if (taskGroupSummary != null)
                        return Ok(taskGroupSummary);

                return NotFound();
            }

            return BadRequest();
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public IActionResult GetValidParents(string id)
        {
            TaskGroupList taskGroupList;

            if (Guid.TryParse(id, out Guid parsedId))
            {
                taskGroupList = _taskManager.GetTaskGroupList(parsedId, true);

                if (taskGroupList.TaskGroupHeaders.Count > 0)
                    return Ok(taskGroupList);

                return NotFound();
            }

            return BadRequest();
        }
    }
}
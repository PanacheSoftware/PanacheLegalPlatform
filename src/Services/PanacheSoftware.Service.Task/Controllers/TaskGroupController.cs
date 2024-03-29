﻿using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.Error;
using PanacheSoftware.Core.Domain.API.Join;
using PanacheSoftware.Core.Domain.API.Task;
using PanacheSoftware.Core.Domain.Task;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Task.Core;
using PanacheSoftware.Service.Task.Manager;

namespace PanacheSoftware.Service.Task.Controllers
{
    [Produces("application/json")]
    [Authorize]
    [Route("[controller]")]
    public class TaskGroupController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITaskManager _taskManager;
        private readonly IUserProvider _userProvider;
        private readonly IAPIHelper _apiHelper;

        public TaskGroupController(IUnitOfWork unitOfWork, IMapper mapper, ITaskManager taskManager, IUserProvider userProvider, IAPIHelper apiHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _taskManager = taskManager;
            _userProvider = userProvider;
            _apiHelper = apiHelper;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                var taskGroupList = await _taskManager.GetTaskGroupListAsync(accessToken);

                if (taskGroupList.TaskGroupHeaders.Count > 0)
                    return Ok(taskGroupList);

                return NotFound();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }
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
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                TaskGroupHeader taskGroupHeader = null;

                if (Guid.TryParse(id, out Guid parsedId))
                {
                    taskGroupHeader = await _unitOfWork.TaskGroupHeaders.GetTaskGroupHeaderWithRelationsAsync(parsedId, false, accessToken);
                }
                else
                {
                    taskGroupHeader = await _unitOfWork.TaskGroupHeaders.GetTaskGroupHeaderWithRelationsAsync(id, false, accessToken);
                }

                if (taskGroupHeader != null)
                {
                    return Ok(_mapper.Map<TaskGroupHead>(taskGroupHeader));
                }

                return NotFound();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TaskGroupHead taskGroupHead)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");

                    if (taskGroupHead.Id == Guid.Empty)
                    {
                        var taskGroupHeader = _mapper.Map<TaskGroupHeader>(taskGroupHead);

                        //if(taskGroupHeader.ChildTaskGroups.Any())
                        //    return StatusCode(StatusCodes.Status400BadRequest,
                        //        new APIErrorMessage(StatusCodes.Status400BadRequest,
                        //        $"Cannot create child task groups when creating parent"));

                        //if (taskGroupHeader.ChildTasks.Any())
                        //    return StatusCode(StatusCodes.Status400BadRequest,
                        //        new APIErrorMessage(StatusCodes.Status400BadRequest,
                        //        $"Cannot create child tasks when creating task group"));

                        //if (!await _taskManager.TaskGroupTeamOkayAsync(taskGroupHeader, accessToken))
                        //    return StatusCode(StatusCodes.Status400BadRequest,
                        //        new APIErrorMessage(StatusCodes.Status400BadRequest,
                        //            $"TaskGroupHead.Id: Can't access '{taskGroupHead.Id}'."));

                        //if (!await _taskManager.TaskGroupParentOkayAsync(taskGroupHeader, accessToken))
                        //    return StatusCode(StatusCodes.Status400BadRequest,
                        //        new APIErrorMessage(StatusCodes.Status400BadRequest,
                        //            $"TaskGroupHead.ParentTaskGroupId: '{taskGroupHead.ParentTaskGroupId}' is invalid."));

                        //var dateCheck = await _taskManager.TaskGroupDatesOkayAsync(taskGroupHeader, accessToken);

                        //if(!dateCheck.Item1)
                        //    return StatusCode(StatusCodes.Status400BadRequest, 
                        //        new APIErrorMessage(StatusCodes.Status400BadRequest,
                        //        dateCheck.Item2));

                        //if (!await _taskManager.SetNewTaskGroupSequenceNoAsync(taskGroupHeader, accessToken))
                        //    return StatusCode(StatusCodes.Status400BadRequest,
                        //        new APIErrorMessage(StatusCodes.Status400BadRequest,
                        //            $"Unable to set task sequence number."));

                        //taskGroupHeader.OriginalCompletionDate = taskGroupHeader.CompletionDate;
                        //taskGroupHeader.CompletedOnDate = DateTime.Parse("01/01/1900");
                        //taskGroupHeader.Completed = false;

                        var taskGroupHeaderCreation = await _taskManager.CreateTaskGroupHeader(taskGroupHeader, accessToken);

                        if(!taskGroupHeaderCreation.Item1)
                            return StatusCode(StatusCodes.Status400BadRequest,
                                new APIErrorMessage(StatusCodes.Status400BadRequest,
                                taskGroupHeaderCreation.Item2));

                        //_unitOfWork.TaskGroupHeaders.Add(taskGroupHeader);

                        //_unitOfWork.Complete();

                        return Created(new Uri($"{Request.Path}/{taskGroupHeader.Id}", UriKind.Relative),
                            _mapper.Map<TaskGroupHead>(taskGroupHeader));
                    }

                    return StatusCode(StatusCodes.Status400BadRequest,
                        new APIErrorMessage(StatusCodes.Status400BadRequest,
                            $"TaskGroupHead.Id: '{taskGroupHead.Id}' is not an empty guid."));
                }
                catch (Exception e)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
                }
            }

            return BadRequest(new APIErrorMessage(StatusCodes.Status400BadRequest, "One or more validation errors occurred.", ModelState));
        }

        [HttpPatch("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Patch(string id, [FromBody]JsonPatchDocument<TaskGroupHead> taskGroupHeadPatch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");

                    if (Guid.TryParse(id, out Guid parsedId))
                    {
                        var taskGroupHeader = _unitOfWork.TaskGroupHeaders.Get(parsedId);

                        if (taskGroupHeader != null)
                        {
                            if (await _taskManager.CanAccessTaskGroupHeaderAsync(taskGroupHeader.Id, accessToken))
                            {
                                var taskGroupHead = _mapper.Map<TaskGroupHead>(taskGroupHeader);

                                taskGroupHeadPatch.ApplyTo(taskGroupHead);

                                //Make sure the Original dates do not get changed
                                taskGroupHead.OriginalCompletionDate = taskGroupHeader.OriginalCompletionDate;
                                taskGroupHead.OriginalStartDate = taskGroupHeader.OriginalStartDate;

                                var dateCheck = _taskManager.TaskGroupDatesOkay(_mapper.Map<TaskGroupHeader>(taskGroupHead), accessToken);

                                if (!dateCheck.Item1)
                                    return StatusCode(StatusCodes.Status400BadRequest,
                                        new APIErrorMessage(StatusCodes.Status400BadRequest,
                                        dateCheck.Item2));

                                if (taskGroupHead.ShortName != taskGroupHeader.ShortName)
                                    return StatusCode(StatusCodes.Status400BadRequest,
                                        new APIErrorMessage(StatusCodes.Status400BadRequest,
                                            $"TaskGroupHeader.ShortName: '{taskGroupHeader.ShortName}' can't be changed."));

                                _mapper.Map(taskGroupHead, taskGroupHeader);

                                if (!await _taskManager.TaskGroupParentOkayAsync(taskGroupHeader, accessToken))
                                    return StatusCode(StatusCodes.Status400BadRequest,
                                        new APIErrorMessage(StatusCodes.Status400BadRequest,
                                            $"TaskGroupHeader.ParentTaskGroupId: '{taskGroupHeader.ParentTaskGroupId}' is invalid."));

                                _unitOfWork.Complete();

                                return Ok();
                            }

                            return StatusCode(StatusCodes.Status400BadRequest,
                                new APIErrorMessage(StatusCodes.Status400BadRequest,
                                    $"TaskGroupHeader.Id: Can't access '{taskGroupHeader.Id}'."));
                        }

                        return NotFound();
                    }

                    return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"id: '{id}' is not a valid guid."));
                }
                catch (Exception e)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
                }
            }

            return BadRequest(new APIErrorMessage(StatusCodes.Status400BadRequest, "One or more validation errors occurred.", ModelState));
        }

        [Route("[action]/{id}/{completionDate}")]
        [HttpPost]
        public async Task<IActionResult> Complete(string id, string completionDate)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");

                    var taskGroupHeaderReadOnly = await _unitOfWork.TaskGroupHeaders.GetTaskGroupHeaderWithRelationsAsync(parsedId, true, accessToken);

                    if(taskGroupHeaderReadOnly != null)
                    {
                        if (await _taskManager.CanAccessTaskGroupHeaderAsync(taskGroupHeaderReadOnly.Id, accessToken))
                        {
                            if (!taskGroupHeaderReadOnly.Completed)
                            {
                                if (await _taskManager.CanCompleteTaskGroupAsync(taskGroupHeaderReadOnly.Id,
                                    accessToken))
                                {
                                    var taskGroupHeader = _unitOfWork.TaskGroupHeaders.Get(taskGroupHeaderReadOnly.Id);

                                    if (taskGroupHeader != null)
                                    {
                                        taskGroupHeader.Completed = true;

                                        var dateTimeFormatString = "yyyyMMddHHmmss";
                                        DateTime.TryParseExact(string.IsNullOrWhiteSpace(completionDate) ? "19000101000000" : completionDate, dateTimeFormatString, null, DateTimeStyles.None, out DateTime convertedDateTime);

                                        taskGroupHeader.CompletedOnDate = convertedDateTime;

                                        var dateCheck = _taskManager.TaskGroupDatesOkay(taskGroupHeader, accessToken);

                                        if (!dateCheck.Item1)
                                            return StatusCode(StatusCodes.Status400BadRequest,
                                                new APIErrorMessage(StatusCodes.Status400BadRequest,
                                                dateCheck.Item2));

                                        _unitOfWork.Complete();

                                        return Ok(_mapper.Map<TaskGroupHead>(taskGroupHeader));
                                    }

                                    return NotFound();
                                }

                                return StatusCode(StatusCodes.Status400BadRequest,
                                    new APIErrorMessage(StatusCodes.Status400BadRequest,
                                        $"TaskGroupHeader.Id: Can't complete."));
                            }

                            return StatusCode(StatusCodes.Status400BadRequest,
                                new APIErrorMessage(StatusCodes.Status400BadRequest,
                                    $"TaskGroupHeader.Id: Already complete."));
                        }

                        return StatusCode(StatusCodes.Status400BadRequest,
                            new APIErrorMessage(StatusCodes.Status400BadRequest,
                                $"TaskGroupHeader.Id: Can't access '{taskGroupHeaderReadOnly.Id}'."));
                    }

                    return NotFound();

                }

                return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"id: '{id}' is not a valid guid."));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> GetMainTaskGroups()
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                var taskGroupList = await _taskManager.GetMainTaskGroupsAsync(accessToken);

                if (taskGroupList.TaskGroupHeaders.Count > 0)
                    return Ok(taskGroupList);

                return NotFound();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> GetTaskGroupSummary()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            try
            {
                var taskGroupList = await _taskManager.GetMainTaskGroupSummarysAsync(accessToken);

                if (taskGroupList.TaskGroupSummarys.Count > 0)
                    return Ok(taskGroupList);

                return NotFound();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetTaskGroupSummary(string id)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    var taskGroupSummary = await _taskManager.GetTaskGroupSummaryAsync(parsedId, accessToken);

                    if (taskGroupSummary != null)
                        return Ok(taskGroupSummary);

                    return NotFound();
                }

                return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"id: '{id}' is not a valid guid."));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }
        }

        [Route("[action]/{id}")]
        [HttpPost]
        public async Task<IActionResult> CreateTaskFromTemplate([FromBody] TaskGroupHead taskGroupHead, string id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");

                    if (taskGroupHead.Id == Guid.Empty)
                    {
                        if (!Guid.TryParse(id, out Guid templateId))
                            return StatusCode(StatusCodes.Status400BadRequest,
                                new APIErrorMessage(StatusCodes.Status400BadRequest,
                                    $"Id: is not a valid guid."));

                        var creationResult = await _taskManager.CreateTaskFromTemplate(taskGroupHead, templateId, accessToken);

                        if (creationResult.Item1 == Guid.Empty)
                            return StatusCode(StatusCodes.Status400BadRequest,
                                new APIErrorMessage(StatusCodes.Status400BadRequest,
                                    $"{creationResult.Item2}"));

                        var taskGroupHeader = await _unitOfWork.TaskGroupHeaders.GetTaskGroupHeaderWithRelationsAsync(creationResult.Item1, true, accessToken);

                        if (taskGroupHeader == null)
                            return StatusCode(StatusCodes.Status400BadRequest,
                                new APIErrorMessage(StatusCodes.Status400BadRequest,
                                    $"Error retrieving Task Group Header: {creationResult.Item1}"));

                        return Created(new Uri($"{Request.Path}/{taskGroupHeader.Id}", UriKind.Relative),
                            _mapper.Map<TaskGroupHead>(taskGroupHeader));
                    }

                    return StatusCode(StatusCodes.Status400BadRequest,
                        new APIErrorMessage(StatusCodes.Status400BadRequest,
                            $"TaskGroupHead.Id: '{taskGroupHead.Id}' is not an empty guid."));
                }
                catch (Exception e)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
                }
            }

            return BadRequest(new APIErrorMessage(StatusCodes.Status400BadRequest, "One or more validation errors occurred.", ModelState));
        }

        //[Route("[action]/{id}")]
        //[HttpGet]
        //public async Task<IActionResult> GetValidParents(string id)
        //{
        //    TaskGroupList taskGroupList;

        //    var accessToken = await HttpContext.GetTokenAsync("access_token");

        //    if (Guid.TryParse(id, out Guid parsedId))
        //    {
        //        taskGroupList = await _taskManager.GetTaskGroupListAsync(accessToken, parsedId, true);

        //        if (taskGroupList.TaskGroupHeaders.Count > 0)
        //            return Ok(taskGroupList);

        //        return NotFound();
        //    }

        //    return BadRequest();
        //}
    }
}
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
using PanacheSoftware.Core.Domain.API.Error;
using PanacheSoftware.Core.Domain.API.Task;
using PanacheSoftware.Core.Domain.Task;
using PanacheSoftware.Service.Task.Core;
using PanacheSoftware.Service.Task.Manager;

namespace PanacheSoftware.Service.Task.Controllers
{
    [Authorize]
    [Route("TaskGroup/Detail")]
    [ApiController]
    public class TaskGroupDetailController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITaskManager _taskManager;

        public TaskGroupDetailController(IUnitOfWork unitOfWork, IMapper mapper, ITaskManager taskManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _taskManager = taskManager;
        }

        //GET: api/Detail/5
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

                if (Guid.TryParse(id, out Guid parsedId))
                {
                    var taskGroupDetail = _unitOfWork.TaskGroupDetails.GetDetail(parsedId, true);

                    if (taskGroupDetail != null)
                    {
                        if (await _taskManager.CanAccessTaskGroupHeaderAsync(taskGroupDetail.TaskGroupHeaderId, accessToken))
                        {
                            return Ok(_mapper.Map<TaskGroupDet>(taskGroupDetail));
                        }

                        return StatusCode(StatusCodes.Status400BadRequest,
                            new APIErrorMessage(StatusCodes.Status400BadRequest,
                                $"TaskGroupDetail.TaskGroupHeaderId: Can't access '{taskGroupDetail.TaskGroupHeaderId}'."));
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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TaskGroupDet taskGroupDet)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");

                    if (taskGroupDet.Id == Guid.Empty)
                    {
                        var taskGroupHeader =
                            _unitOfWork.TaskGroupHeaders.SingleOrDefault(c => c.Id == taskGroupDet.TaskGroupHeaderId,
                                true);

                        if (taskGroupHeader != null)
                        {
                            if (taskGroupHeader.Id != Guid.Empty)
                            {
                                if (await _taskManager.CanAccessTaskGroupHeaderAsync(taskGroupHeader.Id, accessToken))
                                {
                                    //var userId = User.FindFirstValue("sub");

                                    var taskGroupDetail = _mapper.Map<TaskGroupDetail>(taskGroupDet);

                                    _unitOfWork.TaskGroupDetails.Add(taskGroupDetail);

                                    _unitOfWork.Complete();

                                    return Created(new Uri($"{Request.Path}/{taskGroupDetail.Id}", UriKind.Relative),
                                        _mapper.Map<TaskGroupDet>(taskGroupDetail));
                                }

                                return StatusCode(StatusCodes.Status400BadRequest,
                                    new APIErrorMessage(StatusCodes.Status400BadRequest,
                                        $"TaskGroupDet.TaskGroupHeader.Id: Can't access TaskGroupDet.TaskGroupHeader."));
                            }

                            return StatusCode(StatusCodes.Status400BadRequest,
                                new APIErrorMessage(StatusCodes.Status400BadRequest,
                                    $"TaskGroupDet.TaskGroupHeaderId: '{taskGroupDet.TaskGroupHeaderId}' not found."));
                        }

                        return StatusCode(StatusCodes.Status400BadRequest,
                            new APIErrorMessage(StatusCodes.Status400BadRequest,
                                $"TaskGroupDet.TaskGroupHeaderId: '{taskGroupDet.TaskGroupHeaderId}' not found."));
                    }

                    return StatusCode(StatusCodes.Status400BadRequest,
                        new APIErrorMessage(StatusCodes.Status400BadRequest,
                            $"TaskGroupDet.Id: '{taskGroupDet.Id}' is not an empty guid."));
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
        public async Task<IActionResult> Patch(string id, [FromBody]JsonPatchDocument<TaskGroupDet> taskGroupDetPatch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");

                    if (Guid.TryParse(id, out Guid parsedId))
                    {
                        var taskGroupDetail = _unitOfWork.TaskGroupDetails.Get(parsedId);

                        if (taskGroupDetail != null)
                        {
                            if (await _taskManager.CanAccessTaskGroupHeaderAsync(taskGroupDetail.TaskGroupHeaderId,
                                accessToken))
                            {
                                var taskGroupDet = _mapper.Map<TaskGroupDet>(taskGroupDetail);

                                taskGroupDetPatch.ApplyTo(taskGroupDet);

                                _mapper.Map(taskGroupDet, taskGroupDetail);

                                _unitOfWork.Complete();

                                return CreatedAtRoute("Get", new {id = _mapper.Map<TaskGroupDet>(taskGroupDetail).Id},
                                    _mapper.Map<TaskGroupDet>(taskGroupDetail));
                            }

                            return StatusCode(StatusCodes.Status400BadRequest,
                                new APIErrorMessage(StatusCodes.Status400BadRequest,
                                    $"TaskGroupDetail.TaskGroupHeaderId: Can't access TaskGroupDet.TaskGroupHeader."));
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
    }
}
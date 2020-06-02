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
    [Route("api/Task/Detail")]
    [ApiController]
    public class TaskDetailController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TaskDetailController(IUnitOfWork unitOfWork, IMapper mapper)
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
                    TaskDetail taskDetail = _unitOfWork.TaskDetails.GetDetail(parsedId, true);

                    return Ok(_mapper.Map<TaskDet>(taskDetail));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Post([FromBody]TaskDet taskDet)
        {
            try
            {
                if (taskDet.Id == Guid.Empty)
                {
                    TaskHeader taskHeader = _unitOfWork.TaskHeaders.SingleOrDefault(c => c.Id == taskDet.TaskHeaderId, true);

                    if (taskHeader.Id != Guid.Empty)
                    {
                        //var userId = User.FindFirstValue("sub");

                        var taskDetail = _mapper.Map<TaskDetail>(taskDet);

                        _unitOfWork.TaskDetails.Add(taskDetail);

                        _unitOfWork.Complete();

                        return Created(new Uri($"{Request.Path}/{taskDetail.Id}", UriKind.Relative), _mapper.Map<TaskDet>(taskDetail));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<TaskDet> taskDetPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    var taskDetail = _unitOfWork.TaskDetails.Get(parsedId);

                    var taskDet = _mapper.Map<TaskDet>(taskDetail);

                    taskDetPatch.ApplyTo(taskDet);

                    _mapper.Map(taskDet, taskDetail);

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Get", new { id = _mapper.Map<TaskDet>(taskDetail).Id }, _mapper.Map<TaskDet>(taskDetail));
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
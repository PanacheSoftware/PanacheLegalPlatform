﻿using System;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.Error;
using PanacheSoftware.Core.Domain.API.Team;
using PanacheSoftware.Core.Domain.Team;
using PanacheSoftware.Service.Team.Core;

namespace PanacheSoftware.Service.Team.Controllers
{
    [Authorize]
    [Route("Team/Detail")]
    [ApiController]
    public class TeamDetailController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TeamDetailController(IUnitOfWork unitOfWork, IMapper mapper)
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
                    TeamDetail teamDetail = _unitOfWork.TeamDetails.GetDetail(parsedId, true);

                    if(teamDetail != null)
                        return Ok(_mapper.Map<TeamDet>(teamDetail));

                    return NotFound();
                }

                return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"id: '{id}' is not a valid guid."));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]TeamDet teamDet)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (teamDet.Id == Guid.Empty)
                    {
                        TeamHeader teamHeader =
                            _unitOfWork.TeamHeaders.SingleOrDefault(c => c.Id == teamDet.TeamHeaderId, true);

                        if (teamHeader != null)
                        {
                            if (teamHeader.Id != Guid.Empty)
                            {
                                //var userId = User.FindFirstValue("sub");

                                var teamDetail = _mapper.Map<TeamDetail>(teamDet);

                                _unitOfWork.TeamDetails.Add(teamDetail);

                                _unitOfWork.Complete();

                                return Created(new Uri($"{Request.Path}/{teamDetail.Id}", UriKind.Relative),
                                    _mapper.Map<TeamDet>(teamDetail));
                            }

                            return StatusCode(StatusCodes.Status400BadRequest,
                                new APIErrorMessage(StatusCodes.Status400BadRequest,
                                    $"TeamDet.TeamHeaderId: '{teamDet.TeamHeaderId}' can't be an empty Guid."));
                        }

                        return StatusCode(StatusCodes.Status400BadRequest,
                            new APIErrorMessage(StatusCodes.Status400BadRequest,
                                $"TeamDet.TeamHeaderId: '{teamDet.TeamHeaderId}' team header not found."));
                    }

                    return StatusCode(StatusCodes.Status400BadRequest,
                        new APIErrorMessage(StatusCodes.Status400BadRequest,
                            $"TeamDet.Id: '{teamDet.Id}' is not an empty guid."));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<TeamDet> teamDetPatch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Guid.TryParse(id, out Guid parsedId))
                    {
                        //var userId = User.FindFirstValue("sub");

                        TeamDetail teamDetail = _unitOfWork.TeamDetails.Get(parsedId);

                        if (teamDetail != null)
                        {
                            TeamDet teamDet = _mapper.Map<TeamDet>(teamDetail);

                            teamDetPatch.ApplyTo(teamDet);

                            _mapper.Map(teamDet, teamDetail);

                            _unitOfWork.Complete();

                            return CreatedAtRoute("Get", new {id = _mapper.Map<TeamDet>(teamDetail).Id},
                                _mapper.Map<TeamDet>(teamDetail));
                        }

                        return NotFound();
                    }

                    return StatusCode(StatusCodes.Status400BadRequest,
                        new APIErrorMessage(StatusCodes.Status400BadRequest, $"id: '{id}' is not a valid guid."));
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
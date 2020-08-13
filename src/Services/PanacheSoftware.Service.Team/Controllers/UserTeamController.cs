using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.Error;
using PanacheSoftware.Core.Domain.API.Join;
using PanacheSoftware.Core.Domain.API.Team;
using PanacheSoftware.Core.Domain.Join;
using PanacheSoftware.Service.Team.Core;
using PanacheSoftware.Service.Team.Manager;

namespace PanacheSoftware.Service.Team.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UserTeamController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserTeamManager _userTeamManager;

        public UserTeamController(IUnitOfWork unitOfWork, IMapper mapper, IUserTeamManager userTeamManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userTeamManager = userTeamManager;
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
                UserTeamJoinList userTeamJoinList = new UserTeamJoinList();

                userTeamJoinList.UserTeamJoins = _mapper.Map<List<UserTeam>, List<UserTeamJoin>>(_unitOfWork.UserTeams.GetAll(true).ToList());

                if (userTeamJoinList.UserTeamJoins.Any())
                    return Ok(userTeamJoinList);

                return NotFound();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }
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
                   var  userTeam = _unitOfWork.UserTeams.GetUserTeamWithRelations(parsedId, true);

                   if (userTeam != null)
                   {
                       return Ok(_mapper.Map<UserTeamJoin>(userTeam));
                   }

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
        public IActionResult Post([FromBody]UserTeamJoin userTeamJoin)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (userTeamJoin.Id == Guid.Empty)
                    {
                        var userTeam = _mapper.Map<UserTeam>(userTeamJoin);

                        _unitOfWork.UserTeams.Add(userTeam);

                        _unitOfWork.Complete();

                        return Created(new Uri($"{Request.Path}/{userTeam.Id}", UriKind.Relative),
                            _mapper.Map<UserTeamJoin>(userTeam));
                    }

                    return StatusCode(StatusCodes.Status400BadRequest,
                        new APIErrorMessage(StatusCodes.Status400BadRequest,
                            $"UserTeamJoin.Id: '{userTeamJoin.Id}' is not an empty guid."));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<UserTeamJoin> userTeamPatch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Guid.TryParse(id, out Guid parsedId))
                    {
                        var userTeam = _unitOfWork.UserTeams.Get(parsedId);

                        if (userTeam != null)
                        {
                            UserTeamJoin userTeamJoin = _mapper.Map<UserTeamJoin>(userTeam);

                            userTeamPatch.ApplyTo(userTeamJoin);

                            _mapper.Map(userTeamJoin, userTeam);

                            _unitOfWork.Complete();

                            return CreatedAtRoute("Get", new {id = _mapper.Map<UserTeamJoin>(userTeam).Id},
                                _mapper.Map<UserTeamJoin>(userTeam));
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

        [Route("[action]/{id}")]
        [HttpGet]
        public IActionResult GetTeamsForUser(string id)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    TeamList teamList = _userTeamManager.GetTeamsForUser(parsedId);

                    if (teamList.TeamHeaders.Any())
                        return Ok(teamList);

                    return NotFound();
                }

                return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"id: '{id}' is not a valid guid."));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public IActionResult GetUserTeamsForUser(string id)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    var userTeamJoinList = new UserTeamJoinList();

                    userTeamJoinList.UserTeamJoins = _mapper.Map<List<UserTeam>, List<UserTeamJoin>>(_unitOfWork.UserTeams.GetUserTeamsForUser(parsedId, true));

                    if (userTeamJoinList.UserTeamJoins.Any())
                        return Ok(userTeamJoinList);

                    return NotFound();
                }

                return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"id: '{id}' is not a valid guid."));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public IActionResult GetUserTeamsForTeam(string id)
        {
            try
            {
                var userTeamJoinList = new UserTeamJoinList();

                if (Guid.TryParse(id, out Guid parsedId))
                {
                    userTeamJoinList = _userTeamManager.GetUserTeamListForTeam(parsedId);
                }
                else
                {
                    userTeamJoinList = _userTeamManager.GetUserTeamListForTeam(id);
                }

                if (userTeamJoinList.UserTeamJoins.Any())
                    return Ok(userTeamJoinList);

                return NotFound();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }

        }
    }
}
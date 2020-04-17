using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.Join;
using PanacheSoftware.Core.Domain.API.Team;
using PanacheSoftware.Core.Domain.Join;
using PanacheSoftware.Service.Team.Core;
using PanacheSoftware.Service.Team.Manager;

namespace PanacheSoftware.Service.Team.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
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
                
                //foreach (var currentUserTeam in _unitOfWork.UserTeams.GetAll(true))
                //{
                //    userTeamJoinList.UserTeamJoins.Add(_mapper.Map<UserTeamJoin>(currentUserTeam));
                //}

                if (userTeamJoinList.UserTeamJoins.Count() > 0)
                    return Ok(userTeamJoinList);
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
                UserTeam userTeam = new UserTeam();

                if (Guid.TryParse(id, out Guid parsedId))
                {
                    userTeam = _unitOfWork.UserTeams.GetUserTeamWithRelations(parsedId, true);
                }

                if (userTeam != null)
                {
                    return Ok(_mapper.Map<UserTeamJoin>(userTeam));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Post([FromBody]UserTeamJoin userTeamJoin)
        {
            try
            {
                if (userTeamJoin.Id == Guid.Empty)
                {
                    //var userId = User.FindFirstValue("sub");

                    var userTeam = _mapper.Map<UserTeam>(userTeamJoin);

                    _unitOfWork.UserTeams.Add(userTeam);

                    _unitOfWork.Complete();

                    return Created(new Uri($"{Request.Path}/{userTeam.Id}", UriKind.Relative), _mapper.Map<UserTeamJoin>(userTeam));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<UserTeamJoin> userTeamPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    UserTeam userTeam = _unitOfWork.UserTeams.Get(parsedId);

                    UserTeamJoin userTeamJoin = _mapper.Map<UserTeamJoin>(userTeam);

                    userTeamPatch.ApplyTo(userTeamJoin);

                    _mapper.Map(userTeamJoin, userTeam);

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Get", new { id = _mapper.Map<UserTeamJoin>(userTeam).Id }, _mapper.Map<UserTeamJoin>(userTeam));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return BadRequest();
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public IActionResult GetTeamsForUser(string id)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    UserTeamJoinList userTeamJoinList = new UserTeamJoinList();

                    userTeamJoinList.UserTeamJoins = _mapper.Map<List<UserTeam>, List<UserTeamJoin>>(_unitOfWork.UserTeams.GetUserTeamsForUser(parsedId, true));

                    if (userTeamJoinList.UserTeamJoins.Count() > 0)
                        return Ok(userTeamJoinList);

                    return NotFound();
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return BadRequest();
        }

        //[Route("[action]/{id}")]
        //[HttpGet]
        //public IActionResult GetUsersForTeam(string id)
        //{
        //    //UserList userList = new UserList();

        //    //if (Guid.TryParse(id, out Guid parsedId))
        //    //{
        //    //    userList = _userTeamManager.GetUsersForTeam(parsedId);
        //    //}
        //    //else
        //    //{
        //    //    userList = _userTeamManager.GetUsersForTeam(id);
        //    //}

        //    //if (userList.UserProfiles.Any())
        //    //    return Ok(userList);

        //    return NotFound();
        //}

        [Route("[action]/{id}")]
        [HttpGet]
        public IActionResult GetUserTeamsForUser(string id)
        {
            UserTeamJoinList userTeamJoinList = new UserTeamJoinList();

            if (Guid.TryParse(id, out Guid parsedId))
            {
                userTeamJoinList = _userTeamManager.GetUserTeamListForUser(parsedId);
            }

            if (userTeamJoinList.UserTeamJoins.Any())
                return Ok(userTeamJoinList);

            return NotFound();
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public IActionResult GetUserTeamsForTeam(string id)
        {
            UserTeamJoinList userTeamJoinList = new UserTeamJoinList();

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

    }
}
using System;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.Team;
using PanacheSoftware.Core.Domain.Team;
using PanacheSoftware.Service.Team.Core;
using PanacheSoftware.Service.Team.Manager;

namespace PanacheSoftware.Service.Team.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TeamController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITeamManager _teamManager;

        public TeamController(IUnitOfWork unitOfWork, IMapper mapper, ITeamManager teamManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _teamManager = teamManager;
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
                TeamList teamList = _teamManager.GetTeamList();

                if (teamList.TeamHeaders.Count > 0)
                    return Ok(teamList);
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
                TeamHeader teamHeader = null;

                if (Guid.TryParse(id, out Guid parsedId))
                {
                    teamHeader = _unitOfWork.TeamHeaders.GetTeamHeaderWithRelations(parsedId, false);
                }
                else
                {
                    teamHeader = _unitOfWork.TeamHeaders.GetTeamHeaderWithRelations(id, false);
                }

                if (teamHeader != null)
                {
                    return Ok(_mapper.Map<TeamHead>(teamHeader));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Post([FromBody]TeamHead teamHead)
        {
            try
            {
                if (teamHead.Id == Guid.Empty)
                {
                    //var userId = User.FindFirstValue("sub");

                    var teamHeader = _mapper.Map<TeamHeader>(teamHead);

                    _unitOfWork.TeamHeaders.Add(teamHeader);

                    _unitOfWork.Complete();

                    return Created(new Uri($"{Request.Path}/{teamHeader.Id}", UriKind.Relative), _mapper.Map<TeamHead>(teamHeader));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<TeamHead> teamHeadPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    //var userId = User.FindFirstValue("sub");

                    TeamHeader teamHeader = _unitOfWork.TeamHeaders.Get(parsedId);

                    TeamHead teamHead = _mapper.Map<TeamHead>(teamHeader);

                    teamHeadPatch.ApplyTo(teamHead);

                    _mapper.Map(teamHead, teamHeader);

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Get", new { id = _mapper.Map<TeamHead>(teamHeader).Id }, _mapper.Map<TeamHead>(teamHeader));
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
        public IActionResult GetValidParents(string id)
        {
            TeamList teamList;

            if (Guid.TryParse(id, out Guid parsedId))
            {
                teamList = _teamManager.GetTeamList(parsedId, true);

                if (teamList.TeamHeaders.Count > 0)
                    return Ok(teamList);

                return NotFound();
            }

            return BadRequest();
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public IActionResult GetTeamStructure(string id)
        {
            if (Guid.TryParse(id, out Guid parsedId))
            {
                TeamStruct teamStruct = _teamManager.GetTeamStructure(parsedId);

                if (teamStruct.Id != Guid.Empty)
                    return Ok(teamStruct);

                return NotFound();
            }

            return BadRequest();
        }
    }
}

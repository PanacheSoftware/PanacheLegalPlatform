using System;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.Client;
using PanacheSoftware.Core.Domain.Client;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Service.Client.Core;

namespace PanacheSoftware.Service.Client.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DetailController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DetailController(IUnitOfWork unitOfWork, IMapper mapper)
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
                    ClientDetail clientDetail = _unitOfWork.ClientDetails.GetDetail(parsedId, true);

                    return Ok(_mapper.Map<ClientDet>(clientDetail));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Post([FromBody]ClientDet clientDet)
        {
            try
            {
                if (clientDet.Id == Guid.Empty)
                {
                    ClientHeader clientHeader = _unitOfWork.ClientHeaders.SingleOrDefault(c => c.Id == clientDet.ClientHeaderId, true);

                    if (clientHeader.Id != Guid.Empty)
                    {
                        //var userId = User.FindFirstValue("sub");

                        var clientDetail = _mapper.Map<ClientDetail>(clientDet);

                        _unitOfWork.ClientDetails.Add(clientDetail);

                        _unitOfWork.Complete();

                        return Created(new Uri($"{Request.Path}/{clientDetail.Id}", UriKind.Relative), _mapper.Map<ClientDet>(clientDetail));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<ClientDet> clientDetPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    //var userId = User.FindFirstValue("sub");

                    ClientDetail clientDetail = _unitOfWork.ClientDetails.Get(parsedId);

                    ClientDet clientDet = _mapper.Map<ClientDet>(clientDetail);

                    clientDetPatch.ApplyTo(clientDet);

                    _mapper.Map(clientDet, clientDetail);

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Get", new { id = _mapper.Map<ClientDet>(clientDetail).Id }, _mapper.Map<ClientDet>(clientDetail));
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
using System;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.Client;
using PanacheSoftware.Core.Domain.API.Error;
using PanacheSoftware.Core.Domain.Client;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Service.Client.Core;

namespace PanacheSoftware.Service.Client.Controllers
{
    [Authorize]
    [Route("Client/[controller]")]
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

                    if(clientDetail != null)
                        return Ok(_mapper.Map<ClientDet>(clientDetail));

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
        public IActionResult Post([FromBody]ClientDet clientDet)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (clientDet.Id == Guid.Empty)
                    {
                        ClientHeader clientHeader =
                            _unitOfWork.ClientHeaders.SingleOrDefault(c => c.Id == clientDet.ClientHeaderId, true);

                        if (clientHeader.Id != Guid.Empty)
                        {
                            var clientDetail = _mapper.Map<ClientDetail>(clientDet);

                            _unitOfWork.ClientDetails.Add(clientDetail);

                            _unitOfWork.Complete();

                            return Created(new Uri($"{Request.Path}/{clientDetail.Id}", UriKind.Relative),
                                _mapper.Map<ClientDet>(clientDetail));
                        }

                        return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"ClientDet.ClientHeaderId: '{clientDet.ClientHeaderId}' not found."));
                    }

                    return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"ClientDet.Id: '{clientDet.Id}' is not an empty guid."));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<ClientDet> clientDetPatch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Guid.TryParse(id, out Guid parsedId))
                    {
                        ClientDetail clientDetail = _unitOfWork.ClientDetails.Get(parsedId);

                        if (clientDetail != null)
                        {

                            ClientDet clientDet = _mapper.Map<ClientDet>(clientDetail);

                            clientDetPatch.ApplyTo(clientDet);

                            _mapper.Map(clientDet, clientDetail);

                            _unitOfWork.Complete();

                            return Ok();
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
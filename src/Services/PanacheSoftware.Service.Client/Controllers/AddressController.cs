using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using PanacheSoftware.Core.Domain.Client;
using PanacheSoftware.Core.Domain.API.Client;
using PanacheSoftware.Service.Client.Core;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PanacheSoftware.Core.Domain.API.Error;

namespace PanacheSoftware.Service.Client.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddressController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //GET: api/Address/5
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
                    ClientAddress clientAddress = _unitOfWork.ClientAddresses.GetAddress(parsedId, true);

                    if(clientAddress != null)
                        return Ok(_mapper.Map<ClientAddr>(clientAddress));

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
        public IActionResult Post([FromBody]ClientAddr clientAddr)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (clientAddr.Id == Guid.Empty)
                    {
                        ClientContact clientContact =
                            _unitOfWork.ClientContacts.SingleOrDefault(c => c.Id == clientAddr.ClientContactId, true);

                        if (clientContact.Id != Guid.Empty)
                        {
                            //var userId = User.FindFirstValue("sub");

                            var clientAddress = _mapper.Map<ClientAddress>(clientAddr);

                            _unitOfWork.ClientAddresses.Add(clientAddress);

                            _unitOfWork.Complete();

                            return Created(new Uri($"{Request.Path}/{clientAddress.Id}", UriKind.Relative),
                                _mapper.Map<ClientAddr>(clientAddress));
                        }

                        return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"ClientAddr.ClientContactId: '{clientAddr.ClientContactId}' does not exist."));
                    }

                    return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"ClientAddr.Id: '{clientAddr.Id}' is not an empty guid."));
                }
                catch (Exception e)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
                }
            }

            return BadRequest(new APIErrorMessage(StatusCodes.Status400BadRequest, "One or more validation errors occurred.", ModelState));
        }

        [HttpPatch("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<ClientAddr> clientAddrPatch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Guid.TryParse(id, out Guid parsedId))
                    {
                        ClientAddress clientAddress = _unitOfWork.ClientAddresses.Get(parsedId);

                        if (clientAddress != null)
                        {
                            ClientAddr clientAddr = _mapper.Map<ClientAddr>(clientAddress);

                            clientAddrPatch.ApplyTo(clientAddr);

                            _mapper.Map(clientAddr, clientAddress);

                            _unitOfWork.Complete();

                            return Ok();
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

            return BadRequest(new APIErrorMessage(StatusCodes.Status400BadRequest, "One or more validation errors occurred.", ModelState));
        }
    }
}
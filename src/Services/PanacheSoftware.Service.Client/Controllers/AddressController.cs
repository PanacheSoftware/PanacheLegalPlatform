using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using PanacheSoftware.Core.Domain.Client;
using PanacheSoftware.Core.Domain.API.Client;
using PanacheSoftware.Service.Client.Core;
using System.Security.Claims;

namespace PanacheSoftware.Service.Client.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
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

                    return Ok(_mapper.Map<ClientAddr>(clientAddress));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Post([FromBody]ClientAddr clientAddr)
        {
            try
            {
                if (clientAddr.Id == Guid.Empty)
                {
                    ClientContact clientContact = _unitOfWork.ClientContacts.SingleOrDefault(c => c.Id == clientAddr.ClientContactId, true);

                    if (clientContact.Id != Guid.Empty)
                    {
                        //var userId = User.FindFirstValue("sub");

                        var clientAddress = _mapper.Map<ClientAddress>(clientAddr);

                        _unitOfWork.ClientAddresses.Add(clientAddress);

                        _unitOfWork.Complete();

                        return Created(new Uri($"{Request.Path}/{clientAddress.Id}", UriKind.Relative), _mapper.Map<ClientAddr>(clientAddress));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<ClientAddr> clientAddrPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    //var userId = User.FindFirstValue("sub");

                    ClientAddress clientAddress = _unitOfWork.ClientAddresses.Get(parsedId);

                    ClientAddr clientAddr = _mapper.Map<ClientAddr>(clientAddress);

                    clientAddrPatch.ApplyTo(clientAddr);

                    _mapper.Map(clientAddr, clientAddress);

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Get", new { id = _mapper.Map<ClientAddr>(clientAddress).Id }, _mapper.Map<ClientAddr>(clientAddress));
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
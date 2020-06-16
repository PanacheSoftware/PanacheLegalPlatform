using System;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.Client;
using PanacheSoftware.Core.Domain.Client;
using PanacheSoftware.Service.Client.Core;

namespace PanacheSoftware.Service.Client.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ContactController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        //GET: api/Contact/5
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    ClientContact clientContact = _unitOfWork.ClientContacts.SingleOrDefault(c => c.Id == parsedId, true);

                    if (clientContact.Id != Guid.Empty)
                    {
                        return Ok(_mapper.Map<ClientCon>(clientContact));
                    }
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Post([FromBody]ClientCon clientCon)
        {
            try
            {
                if (clientCon.Id == Guid.Empty)
                {
                    ClientHeader clientHeader = _unitOfWork.ClientHeaders.SingleOrDefault(c => c.Id == clientCon.ClientHeaderId, true);

                    if (clientHeader.Id != Guid.Empty)
                    {
                        if (!clientHeader.ClientContacts.Where(cc => cc.Email == clientCon.Email).Any())
                        {
                            //var userId = User.FindFirstValue("sub");

                            var clientContact = _mapper.Map<ClientContact>(clientCon);

                            _unitOfWork.ClientContacts.Add(clientContact);

                            _unitOfWork.Complete();

                            return Created(new Uri($"{Request.Path}/{clientContact.Id}", UriKind.Relative), _mapper.Map<ClientCon>(clientContact));
                        }
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<ClientCon> clientConPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    //var userId = User.FindFirstValue("sub");

                    ClientContact clientContact = _unitOfWork.ClientContacts.Get(parsedId);

                    ClientCon clientCon = _mapper.Map<ClientCon>(clientContact);

                    clientConPatch.ApplyTo(clientCon);

                    _mapper.Map(clientCon, clientContact);

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Get", new { id = _mapper.Map<ClientCon>(clientContact).Id }, _mapper.Map<ClientCon>(clientContact));
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
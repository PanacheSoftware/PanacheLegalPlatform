using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using PanacheSoftware.Core.Domain.API.Client;
using PanacheSoftware.Core.Domain.Client;
using PanacheSoftware.Service.Client.Core;
using System.Security.Claims;
using PanacheSoftware.Http;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Service.Client.Manager;
using Microsoft.AspNetCore.Http;
using PanacheSoftware.Core.Domain.API.Error;
using PanacheSoftware.Core.Domain.Core;
using System.Threading.Tasks;
using PanacheSoftware.Core.Domain.API;
using System.Collections.Generic;

namespace PanacheSoftware.Service.Client.Controllers
{
    [Produces("application/json")]
    [Authorize]
    [Route("[controller]")]
    public class ClientController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClientManager _clientManager;

        public ClientController(IUnitOfWork unitOfWork, IMapper mapper, IClientManager clientManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _clientManager = clientManager;
        }

        /// <summary>
        /// Gets all Clients.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///        "id": 1,
        ///        "name": "Item1",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks>
        /// <returns>A collection of Client Headers</returns>
        /// <response code="200">If 1-n Clients were found</response>
        /// <response code="401">If the user is not authorised for this function</response> 
        /// <response code="404">If no client Headers found</response> 
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public IActionResult Get()
        {
            try
            {
                ClientList clientList = new ClientList();

                foreach (var currentClient in _unitOfWork.ClientHeaders.GetAll(true))
                {
                    clientList.ClientHeaders.Add(_mapper.Map<ClientHead>(currentClient));
                }

                if (clientList.ClientHeaders.Count > 0)
                    return Ok(clientList);

                return NotFound();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }
        }

        /// <summary>
        /// Gets a specified client.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///        "id": 1,
        ///        "name": "Item1",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks>
        /// <returns>A collection of Client Headers</returns>
        /// <response code="200">If 1-n Clients were found</response>
        /// <response code="401">If the user is not authorised for this function</response> 
        /// <response code="404">If no client Headers found</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public IActionResult Get(string id)
        {
            try
            {
                ClientHeader clientHeader = null;

                if (Guid.TryParse(id, out Guid parsedId))
                {
                    clientHeader = _unitOfWork.ClientHeaders.GetClientHeaderWithRelations(parsedId, true);
                }
                else
                {
                    clientHeader = _unitOfWork.ClientHeaders.GetClientHeaderWithRelations(id, true);
                }

                if (clientHeader != null)
                    return Ok(_mapper.Map<ClientHead>(clientHeader));

                return NotFound();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            } 
        }

        [HttpPost]
        public IActionResult Post([FromBody]ClientHead clientHead)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (clientHead.Id == Guid.Empty)
                    {
                        if(_clientManager.ClientShortNameExists(clientHead.ShortName))
                            return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"ClientHead.ShortName: '{clientHead.ShortName}' already exists."));

                        var clientHeader = _mapper.Map<ClientHeader>(clientHead);

                        _unitOfWork.ClientHeaders.Add(clientHeader);

                        _unitOfWork.Complete();

                        return Created(new Uri($"{Request.Path}/{clientHeader.Id}", UriKind.Relative),
                            _mapper.Map<ClientHead>(clientHeader));
                    }

                    return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"ClientHead.Id: '{clientHead.Id}' is not an empty guid."));
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
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<ClientHead> clientHeadPatch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Guid.TryParse(id, out Guid parsedId))
                    {
                        ClientHeader clientHeader = _unitOfWork.ClientHeaders.Get(parsedId);

                        if (clientHeader != null)
                        {
                            ClientHead clientHead = _mapper.Map<ClientHead>(clientHeader);

                            clientHeadPatch.ApplyTo(clientHead);

                            if (_clientManager.ClientShortNameExists(clientHead.ShortName, parsedId))
                                return StatusCode(StatusCodes.Status400BadRequest, new APIErrorMessage(StatusCodes.Status400BadRequest, $"ClientHead.ShortName: '{clientHead.ShortName}' already exists."));

                            _mapper.Map(clientHead, clientHeader);

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

        [Route("[action]/{id}")]
        [HttpGet]
        public IActionResult GetClientSummary(string id)
        {
            var clientSummary = _clientManager.GetClientSummary(id);

            if (clientSummary != null)
                return Ok(clientSummary);

            return NotFound();
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> GetPaginatedClients(int? pageNumber, string sortField, string sortOrder, int pageSize)
        {
            var paginationModel = new Pagination(pageNumber, sortField, sortOrder);

            try
            {
                var clientHeaderList = await _unitOfWork.ClientHeaders.GetPaginatedListAsync(paginationModel, pageSize);

                var clientHeadList = new Paginated<ClientHead>(_mapper.Map<List<ClientHeader>, List<ClientHead>>(clientHeaderList.Items), clientHeaderList.Items.Count, clientHeaderList.PageIndex, pageSize, clientHeaderList.TotalPages);

                if (clientHeadList.Items.Count > 0)
                    return Ok(clientHeadList);

                return NotFound();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new APIErrorMessage(StatusCodes.Status500InternalServerError, e.Message));
            }
        }
    }
}
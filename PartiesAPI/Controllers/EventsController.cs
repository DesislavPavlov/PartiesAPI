using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using Org.BouncyCastle.Security;
using PartiesAPI.DTO;
using PartiesAPI.Exceptions;
using PartiesAPI.Models;
using PartiesAPI.Services.EventService;

namespace PartiesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _service;

        public EventsController(IEventService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<EventDTO>> CreateEvent([FromBody] EventDTO eventDTO)
        {
            ActionResult result;

            try
            {
                var @event = await _service.CreateEvent(eventDTO);

                result = Ok(@event);
            }
            catch (BadHttpRequestException ex)
            {
                result = BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                result = NotFound(ex.Message);
            }
            catch (DatabaseOperationException ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventDTO>> GetEventById(int id)
        {
            ActionResult result;

            try
            {
                var @event = await _service.GetEventById(id);

                result = Ok(@event);
            }
            catch (NotFoundException ex)
            {
                result = NotFound(ex.Message);
            }
            catch (DatabaseOperationException ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpPost("{eventId}/joinevent/{userId}")]
        public async Task<ActionResult<EventDTO>> JoinEvent(int eventId, int userId)
        {
            ActionResult result;

            try
            {
                var eventParticipant = await _service.JoinEvent(eventId, userId);

                result = Ok(eventParticipant);
            }
            catch (NotFoundException ex)
            {
                result = NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                result = Conflict(ex.Message);
            }
            catch (DatabaseOperationException ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpGet("{eventId}/participants")]
        public async Task<ActionResult<List<EventParticipantDTO>>> GetEventParticipantsByEventId(int eventId)
        {
            ActionResult result;

            try
            {
                List<EventParticipantDTO> eventParticipantDTOs = await _service.GetEventParticipantsByEventId(eventId);

                result = Ok(eventParticipantDTOs);
            }
            catch (NotFoundException ex)
            {
                result = NotFound(ex.Message);
            }
            catch (DatabaseOperationException ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpPut("{eventId}/organizer/{userId}")]
        public async Task<ActionResult> ChangeOrganizer(int eventId, int  userId)
        {
            ActionResult result;

            try
            {
                await _service.ChangeOrganizer(eventId, userId);

                result = NoContent();
            }
            catch(NotFoundException ex)
            {
                result = NotFound(ex.Message);
            }
            catch(InvalidOperationException ex)
            {
                result = Conflict(ex.Message);
            }
            catch (DatabaseOperationException ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return result;
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Security;
using PartiesAPI.DTO;
using PartiesAPI.Models;
using PartiesAPI.Services;

namespace PartiesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly PartiesService _service;

        public EventsController(PartiesService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<EventDTO>> CreateEvent([FromBody] EventDTO eventDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserDTO organizer = await _service.GetUserById(eventDTO.OrganizerId);

            if (organizer == null)
            {
                return BadRequest("Organizer does not exist!");
            }

            var @event = await _service.CreateEvent(eventDTO);

            return Ok(@event);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventDTO>> GetEventById(int id)
        {
            var @event = await _service.GetEventById(id);

            if (@event == null)
            {
                return BadRequest($"Event with ID of '{id}' does not exist!");
            }

            return Ok(@event);
        }

        [HttpPost("{id}/joinevent/{userId}")]
        public async Task<ActionResult<EventDTO>> JoinEvent(int id, int userId)
        {
            var eventParticipant = await _service.JoinEvent(id, userId);

            if (eventParticipant == null)
            {
                return BadRequest($"Either an event with ID of '{id}' or a user with ID of '{userId}' does not exist!");
            }

            return Ok(eventParticipant);
        }
    }
}

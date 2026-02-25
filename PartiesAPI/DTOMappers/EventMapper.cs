using Microsoft.EntityFrameworkCore;
using PartiesAPI.DTO;
using PartiesAPI.Models;

namespace PartiesAPI.DTOMappers
{
    public class EventMapper
    {
        public EventDTO ToDTO(Event @event)
        {
            return new EventDTO()
            {
                EventId = @event.EventId,
                Name = @event.Name,
                Location = @event.Location,
                StartDate = @event.StartDate,
                EndDate = @event.EndDate,
                OrganizerId = @event.OrganizerId,
            };
        }

        public Event ToEvent(EventDTO eventDTO)
        {
            return new Event()
            {
                Name = eventDTO.Name,
                Location = eventDTO.Location,
                StartDate = eventDTO.StartDate,
                EndDate = eventDTO.EndDate,
                OrganizerId = eventDTO.OrganizerId,
            };
        }
    }
}

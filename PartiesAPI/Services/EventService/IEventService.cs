using PartiesAPI.DTO;

namespace PartiesAPI.Services.EventService
{
    public interface IEventService
    {
        public Task<EventDTO> CreateEvent(EventDTO eventDTO);
        public Task<EventDTO> GetEventById(int id);
        public Task<EventParticipantDTO> JoinEvent(int eventId, int userId);
        public Task<List<EventParticipantDTO>> GetEventParticipantsByEventId(int evendId);
    }
}

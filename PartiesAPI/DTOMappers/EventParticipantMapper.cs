using PartiesAPI.DTO;
using PartiesAPI.Models;

namespace PartiesAPI.DTOMappers
{
    public class EventParticipantMapper
    {
        public EventParticipantDTO ToDTO(EventParticipant eventParticipant)
        {
            return new EventParticipantDTO()
            {
                EventId = eventParticipant.EventId,
                UserId = eventParticipant.UserId,
                JoinDate = eventParticipant.JoinDate,
            };
        }

        public EventParticipant ToEventParticipant(EventParticipantDTO eventParticipantDTO)
        {
            return new EventParticipant()
            {
                EventId = eventParticipantDTO.EventId,
                UserId = eventParticipantDTO.UserId,
                JoinDate = eventParticipantDTO.JoinDate,
            };
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PartiesAPI.Data;
using PartiesAPI.DTO;
using PartiesAPI.DTOMappers;
using PartiesAPI.Exceptions;
using PartiesAPI.Models;
using PartiesAPI.Services.UserService;
using PartiesAPI.Utils;

namespace PartiesAPI.Services.EventService
{
    public class EventService : IEventService
    {
        private readonly PartyDbContext _context;
        private readonly EventMapper _mapper;
        private readonly EventParticipantMapper _eventParticipantMapper;

        public EventService(PartyDbContext context, EventMapper mapper, EventParticipantMapper eventParticipantMapper)
        {
            _context = context;
            _mapper = mapper;
            _eventParticipantMapper = eventParticipantMapper;
        }

        public async Task<EventDTO> CreateEvent(EventDTO eventDTO)
        {
            // Validate model
            if (
                string.IsNullOrEmpty(eventDTO.Name) ||
                string.IsNullOrEmpty(eventDTO.Location) ||
                eventDTO.StartDate == default ||
                eventDTO.EndDate == default ||
                eventDTO.OrganizerId == 0
               )
            {
                throw new BadHttpRequestException(ExceptionMessages.InvalidEventModel);
            }

            // Validate organizer
            if (await _context.Users.AnyAsync(u => u.UserId == eventDTO.OrganizerId) == false)
            {
                throw new NotFoundException(string.Format(ExceptionMessages.UserNotFound, eventDTO.OrganizerId));
            }

            // Create & save event
            Event @event = _mapper.ToEvent(eventDTO);

            try
            {
                await _context.Events.AddAsync(@event);

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DatabaseOperationException(ExceptionMessages.DatabaseError);
            }

            return eventDTO;
        }

        public async Task<EventDTO> GetEventById(int id)
        {
            // Get event & validate
            Event @event;

            try
            {
                @event = await _context.Events.SingleOrDefaultAsync(e => e.EventId == id);
            }
            catch (Exception)
            {
                throw new DatabaseOperationException(ExceptionMessages.DatabaseError);
            }

            if (@event == null)
            {
                throw new NotFoundException(string.Format(ExceptionMessages.EventNotFound, id));
            }

            // Make an eventDTO to return to user
            EventDTO @eventDto = _mapper.ToDTO(@event);

            return @eventDto;
        }

        public async Task<EventParticipantDTO> JoinEvent(int eventId, int userId)
        {
            // Validate event & user
            // Check if user already joined or organizer
            bool eventExists, userExists, userAlreadyParticipant, userAlreadyOrganizer;

            try
            {
                eventExists = await _context.Events.AnyAsync(e => e.EventId == eventId);

                userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

                userAlreadyParticipant = await _context.EventParticipants.AnyAsync(ep => ep.EventId == eventId && ep.UserId == userId);

                userAlreadyOrganizer = await _context.Users.AnyAsync(u => u.UserId == userId && u.OrganizedEvents.Select(e => e.EventId).Contains(eventId));
            }
            catch (Exception)
            {
                throw new DatabaseOperationException(ExceptionMessages.DatabaseError);
            }

            if (!eventExists)
            {
                throw new NotFoundException(string.Format(ExceptionMessages.EventNotFound, eventId));
            }

            if (!userExists)
            {
                throw new NotFoundException(string.Format(ExceptionMessages.UserNotFound, userId));
            }

            if (userAlreadyParticipant)
            {
                throw new InvalidOperationException(string.Format(ExceptionMessages.UserAlreadyParticipant, userId, eventId));
            }

            if (userAlreadyOrganizer)
            {
                throw new InvalidOperationException(string.Format(ExceptionMessages.UserAlreadyOrganizer, userId, eventId));
            }

            // Create eventParticipant to save & DTO to return
            EventParticipant eventParticipant = new EventParticipant()
            {
                EventId = eventId,
                UserId = userId,
                JoinDate = DateTime.UtcNow,
            };

            EventParticipantDTO eventParticipantDTO = _eventParticipantMapper.ToDTO(eventParticipant);

            try
            {
                await _context.EventParticipants.AddAsync(eventParticipant);

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DatabaseOperationException(ExceptionMessages.DatabaseError);
            }

            return eventParticipantDTO;
        }

        public async Task<List<EventParticipantDTO>> GetEventParticipantsByEventId(int evendId)
        {
            // Get event participants
            List<EventParticipant> eventParticipants;

            try
            {
                await GetEventById(evendId);
                eventParticipants = await _context.EventParticipants.Where(ep => ep.EventId == evendId).ToListAsync();
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch (Exception)
            {
                throw new DatabaseOperationException(ExceptionMessages.DatabaseError);
            }

            // Make eventpartcipantDTOs and return
            List<EventParticipantDTO> eventParticipantsDTOs = new List<EventParticipantDTO>();

            foreach (EventParticipant eventParticipant in eventParticipants)
            {
                EventParticipantDTO eventParticipantDTO = _eventParticipantMapper.ToDTO(eventParticipant);

                eventParticipantsDTOs.Add(eventParticipantDTO);
            }

            return eventParticipantsDTOs;
        }

        public async Task ChangeOrganizer(int eventId, int userId)
        {
            // Validate event & user
            // Check if user already organizer
            bool eventExists, userExists, userAlreadyOrganizer;

            try
            {
                eventExists = await _context.Events.AnyAsync(e => e.EventId == eventId);

                userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

                userAlreadyOrganizer = await _context.Users.AnyAsync(u => u.UserId == userId && u.OrganizedEvents.Select(e => e.EventId).Contains(eventId));
            }
            catch (Exception)
            {
                throw new DatabaseOperationException(ExceptionMessages.DatabaseError);
            }

            if (!eventExists)
            {
                throw new NotFoundException(string.Format(ExceptionMessages.EventNotFound, eventId));
            }

            if (!userExists)
            {
                throw new NotFoundException(string.Format(ExceptionMessages.UserNotFound, userId));
            }

            if (userAlreadyOrganizer)
            {
                throw new InvalidOperationException(string.Format(ExceptionMessages.UserAlreadyOrganizer, userId, eventId));
            }

            // Change event's organizerId and save
            try
            {
                Event @event = await _context.Events.SingleOrDefaultAsync(e => e.EventId == eventId);

                @event.OrganizerId = userId;

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DatabaseOperationException(ExceptionMessages.DatabaseError);
            }
        }
    }
}

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
        private readonly IUserService _userService;
        private readonly EventMapper _mapper;

        public EventService(PartyDbContext context, IUserService userService, EventMapper mapper)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
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
            await _userService.GetUserById(eventDTO.OrganizerId);

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
            // Check if user already joined
            bool eventExists, userExists, userAlreadyParticipant, userAlreadyOrganizer;

            try
            {
                eventExists = await _context.Events.AnyAsync(e => e.EventId == eventId);

                userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

                userAlreadyParticipant = await _context.EventParticipants.AnyAsync(ep => ep.EventId == eventId && ep.UserId == userId);

                var user = await _userService.GetUserById(userId);
                userAlreadyOrganizer = user.OrganizedEventIds.Any(id => id == eventId);
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
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

            EventParticipantDTO eventParticipantDTO = new EventParticipantDTO()
            {
                EventId = eventId,
                UserId = userId,
                JoinDate = DateTime.UtcNow,
            };

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
    }
}

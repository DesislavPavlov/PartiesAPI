using Microsoft.EntityFrameworkCore;
using PartiesAPI.Data;
using PartiesAPI.DTO;
using PartiesAPI.Models;

namespace PartiesAPI.Services
{
    public class PartiesService
    {
        private readonly PartyDbContext _context;

        public PartiesService(PartyDbContext context)
        {
            _context = context;
        }


        // Events
        public async Task<EventDTO> CreateEvent(EventDTO eventDTO)
        {
            Event @event = new Event()
            {
                EventId = eventDTO.EventId,
                Name = eventDTO.Name,
                Location = eventDTO.Location,
                StartDate = eventDTO.StartDate,
                EndDate = eventDTO.EndDate,
                OrganizerId = eventDTO.OrganizerId,
            };

            await _context.Events.AddAsync(@event);

            await _context.SaveChangesAsync();
            
            return eventDTO;
        }

        public async Task<EventDTO> GetEventById(int id)
        {
            var @event = await _context.Events.SingleOrDefaultAsync(e => e.EventId == id);

            if (@event == null)
            {
                return null;
            }

            EventDTO @eventDto = new EventDTO()
            {
                EventId = @event.EventId,
                Name = @event.Name,
                Location = @event.Location,
                StartDate = @event.StartDate,
                EndDate = @event.EndDate,
                OrganizerId = @event.OrganizerId,
            };

            return @eventDto;
        }

        public async Task<EventParticipantDTO> JoinEvent(int eventId, int userId)
        {
            bool eventExists = await _context.Events.AnyAsync(e => e.EventId == eventId);

            bool userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
            
            if (!eventExists || !userExists)
            {
                return null;
            }

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

            await _context.EventParticipants.AddAsync(eventParticipant);

            await _context.SaveChangesAsync();
            
            return eventParticipantDTO;
        }
        

        // Users
        public async Task<List<UserDTO>> GetAllUsers()
        {
            List<User> users = await _context.Users.Include(u => u.OrganizedEvents).ToListAsync();

            if (users == null || users.Count == 0)
            {
                return null;
            }

            List<UserDTO> userDTOs = new List<UserDTO>();

            foreach (var user in users)
            {
                UserDTO userDTO = new UserDTO()
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    OrganizedEventIds = user.OrganizedEvents.Select(e => e.EventId).ToList(),
                };

                userDTOs.Add(userDTO);
            }

            return userDTOs;
        }
        public async Task<UserDTO> GetUserById(int id)
        {
            var user = await _context.Users.Include(u => u.OrganizedEvents).SingleOrDefaultAsync(u => u.UserId == id);

            UserDTO userDTO = new UserDTO()
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                OrganizedEventIds = user.OrganizedEvents.Select(e => e.EventId).ToList(),
            };

            return userDTO;
        }
        public async Task<UserDTO> CreateUser(UserDTO userDTO)
        {
            User user = new User()
            {
                UserId = userDTO.UserId,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.Email,
                OrganizedEvents = await _context.Events.Where(user => userDTO.OrganizedEventIds.Contains(user.EventId)).ToListAsync(),
            };

            await _context.Users.AddAsync(user);
            
            await _context.SaveChangesAsync();
            
            return userDTO;
        }
        public async Task<bool> DeleteUser(int id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return false;
            }

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

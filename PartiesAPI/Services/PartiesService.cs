using Microsoft.EntityFrameworkCore;
using PartiesAPI.Data;
using PartiesAPI.DTO;
using PartiesAPI.Models;
using PartiesAPI.Exceptions;
using System.Runtime.ConstrainedExecution;
using Org.BouncyCastle.Security;
using System.Text.RegularExpressions;

namespace PartiesAPI.Services
{
    public class PartiesService
    {
        private readonly PartyDbContext _context;

        public PartiesService(PartyDbContext context)
        {
            _context = context;
        }

        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);


        // Events
        public async Task<EventDTO> CreateEvent(EventDTO eventDTO)
        {
            // Validate model
            if (
                string.IsNullOrEmpty(eventDTO.Name) ||
                string.IsNullOrEmpty(eventDTO.Location) ||
                eventDTO.StartDate == default(DateTime) ||
                eventDTO.EndDate == default(DateTime) ||
                eventDTO.OrganizerId == 0
               )
            {
                throw new BadHttpRequestException("Invalid event format!");
            }

            // Validate organizer
            await GetUserById(eventDTO.OrganizerId);

            // Create & save event
            Event @event = new Event()
            {
                Name = eventDTO.Name,
                Location = eventDTO.Location,
                StartDate = eventDTO.StartDate,
                EndDate = eventDTO.EndDate,
                OrganizerId = eventDTO.OrganizerId,
            };

            try
            {
                await _context.Events.AddAsync(@event);

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DatabaseOperationException("Something went wrong while trying to save your new event!");
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
                throw new DatabaseOperationException("Something went wrong while trying to find your event!");
            }

            if (@event == null)
            {
                throw new NotFoundException($"Event with ID of '{id}' does not exist!");
            }

            // Make an eventDTO to return to user
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
            // Validate event & user
            // Check if user already joined
            bool eventExists, userExists, userAlreadyParticipant;

            try
            {
                eventExists = await _context.Events.AnyAsync(e => e.EventId == eventId);

                userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

                userAlreadyParticipant = await _context.EventParticipants.AnyAsync(ep => ep.EventId == eventId && ep.UserId == userId);
            }
            catch (Exception)
            {
                throw new DatabaseOperationException("Something went wrong while trying to find your event & user!");
            }
            
            if (!eventExists || !userExists)
            {
                throw new NotFoundException($"Either an event with ID of '{eventId}', or a user with ID of '{userId}' does not exist!");
            }

            if (userAlreadyParticipant)
            {
                throw new InvalidOperationException($"A user with ID of '{userId}' already participates in the event with ID of '{eventId}'!");
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
                throw new DatabaseOperationException("Something went wrong while trying to add your participant to an event!");
            }

            return eventParticipantDTO;
        }
        

        // Users
        public async Task<List<UserDTO>> GetAllUsers()
        {
            // Get users
            List<User> users;

            try
            {
                users = await _context.Users.Include(u => u.OrganizedEvents).ToListAsync();
            }
            catch (Exception)
            {
                throw new DatabaseOperationException("Something went wrong while trying to get all users!");
            }

            // Make users userDTOs to return
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
            // Get & validate user
            User user;

            try
            {
                user = await _context.Users.Include(u => u.OrganizedEvents).SingleOrDefaultAsync(u => u.UserId == id);
            }
            catch (Exception)
            {
                throw new DatabaseOperationException("Something went wrong when trying to find your user!");
            }

            if (user == null)
            {
                throw new NotFoundException($"A user with ID of '{id}' does not exist!");
            }

            // Make userDTO to return
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
            // Validate model
            if (
                string.IsNullOrEmpty(userDTO.FirstName) ||
                string.IsNullOrEmpty(userDTO.LastName)
               )
            {
                throw new BadHttpRequestException("Invalid user format!");
            }

            // Validate email
            await ValidateEmail(userDTO.Email);

            // Create & save user
            User user = new User()
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.Email,
                OrganizedEvents = await _context.Events.Where(user => userDTO.OrganizedEventIds.Contains(user.EventId)).ToListAsync(),
            };

            try
            {
                await _context.Users.AddAsync(user);

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DatabaseOperationException("Something went wrong while trying to save your user!");
            }
            
            return userDTO;
        }
        public async Task DeleteUser(int id)
        {
            // Validate user
            User user;

            try
            {
                user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == id);
            }
            catch (Exception)
            {
                throw new DatabaseOperationException("Something went wrong while trying to get your user.");
            }

            if (user == null)
            {
                throw new NotFoundException($"A user with ID of '{id}' does not exist!");
            }

            // Delete user
            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DatabaseOperationException("Something went wrong while trying to delete your user.");
            }
        }
        private async Task ValidateEmail(string email)
        {
            // Validate email format
            if (string.IsNullOrEmpty(email) || EmailRegex.IsMatch(email) == false)
            {
                throw new BadHttpRequestException("Invalid email format!");
            }

            // Validate email being taken
            bool emailTaken;

            try
            {
                emailTaken = await _context.Users.AnyAsync(u => u.Email == email);
            }
            catch (Exception)
            {
                throw new DatabaseOperationException("Something went wrong while trying to access the database! We will be fixing the problem ASAP!");
            }

            if (emailTaken)
            {
                throw new InvalidOperationException("Email already taken!");
            }
        }
    }
}

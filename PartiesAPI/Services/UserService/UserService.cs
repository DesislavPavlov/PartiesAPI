using Microsoft.EntityFrameworkCore;
using PartiesAPI.Data;
using PartiesAPI.DTO;
using PartiesAPI.DTOMappers;
using PartiesAPI.Exceptions;
using PartiesAPI.Models;
using PartiesAPI.Utils;
using System.Text.RegularExpressions;

namespace PartiesAPI.Services.UserService
{
    public class UserService : IUserService
    {
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly PartyDbContext _context;
        private readonly UserMapper _mapper;

        public UserService(PartyDbContext context, UserMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

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
                throw new DatabaseOperationException(ExceptionMessages.DatabaseError);
            }

            // Make users userDTOs to return
            List<UserDTO> userDTOs = new List<UserDTO>();

            foreach (var user in users)
            {
                UserDTO userDTO = _mapper.ToDTO(user);

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
                throw new DatabaseOperationException(ExceptionMessages.DatabaseError);
            }

            if (user == null)
            {
                throw new NotFoundException(string.Format(ExceptionMessages.UserNotFound, id));
            }

            // Make userDTO to return
            UserDTO userDTO = _mapper.ToDTO(user);

            return userDTO;
        }
        public async Task<UserDTO> CreateUser(UserCreateDTO userCreateDTO)
        {
            // Validate model
            if (
                string.IsNullOrEmpty(userCreateDTO.FirstName) ||
                string.IsNullOrEmpty(userCreateDTO.LastName)
               )
            {
                throw new BadHttpRequestException(ExceptionMessages.InvalidUserModel);
            }

            // Validate email
            await ValidateEmail(userCreateDTO.Email);

            // Create & save user
            User user = _mapper.ToUser(userCreateDTO);

            try
            {
                await _context.Users.AddAsync(user);

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DatabaseOperationException(ExceptionMessages.DatabaseError);
            }

            // Return mapped to userDTO
            return _mapper.ToDTO(user);
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
                throw new DatabaseOperationException(ExceptionMessages.DatabaseError);
            }

            if (user == null)
            {
                throw new NotFoundException(string.Format(ExceptionMessages.UserNotFound, id));
            }

            // Delete user & save changes
            _context.Users.Remove(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new OrganizerDeletionException(string.Format(ExceptionMessages.CannotDeleteOrganizer, id));
            }
            catch (Exception)
            {
                throw new DatabaseOperationException(ExceptionMessages.DatabaseError);
            }
        }
        private async Task ValidateEmail(string email)
        {
            // Validate email format
            if (string.IsNullOrEmpty(email) || EmailRegex.IsMatch(email) == false)
            {
                throw new BadHttpRequestException(ExceptionMessages.InvalidEmailFormat);
            }

            // Validate email being taken
            bool emailTaken;

            try
            {
                emailTaken = await _context.Users.AnyAsync(u => u.Email == email);
            }
            catch (Exception)
            {
                throw new DatabaseOperationException(ExceptionMessages.DatabaseError);
            }

            if (emailTaken)
            {
                throw new InvalidOperationException(ExceptionMessages.EmailTaken);
            }
        }
    }
}

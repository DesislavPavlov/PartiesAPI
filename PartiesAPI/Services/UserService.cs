﻿using Microsoft.EntityFrameworkCore;
using PartiesAPI.Data;
using PartiesAPI.DTO;
using PartiesAPI.Exceptions;
using PartiesAPI.Models;
using PartiesAPI.Utils;
using System.Text.RegularExpressions;

namespace PartiesAPI.Services
{
    public class UserService
    {
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly PartyDbContext _context;

        public UserService(PartyDbContext context)
        {
            _context = context;
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
                throw new DatabaseOperationException(ExceptionMessages.DatabaseError);
            }

            if (user == null)
            {
                throw new NotFoundException(string.Format(ExceptionMessages.UserNotFound, id));
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
                throw new BadHttpRequestException(ExceptionMessages.InvalidUserModel);
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
                throw new DatabaseOperationException(ExceptionMessages.DatabaseError);
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
                throw new DatabaseOperationException(ExceptionMessages.DatabaseError);
            }

            if (user == null)
            {
                throw new NotFoundException(string.Format(ExceptionMessages.UserNotFound, id));
            }

            // Delete user
            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
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

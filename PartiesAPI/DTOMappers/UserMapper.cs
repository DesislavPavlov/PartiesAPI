using Microsoft.EntityFrameworkCore;
using PartiesAPI.Data;
using PartiesAPI.DTO;
using PartiesAPI.Models;

namespace PartiesAPI.DTOMappers
{
    public class UserMapper
    {
        public UserDTO ToDTO(User user)
        {
            return new UserDTO()
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                OrganizedEventIds = user.OrganizedEvents.Select(e => e.EventId).ToList(),
            };
        }

        public User ToUser(UserCreateDTO userDTO)
        {
            return new User()
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.Email,
                OrganizedEvents = new List<Event>(),
            };
        }
    }
}

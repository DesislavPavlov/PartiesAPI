using PartiesAPI.Models;

namespace PartiesAPI.DTO
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public ICollection<int> OrganizedEventIds { get; set; }
    }
}

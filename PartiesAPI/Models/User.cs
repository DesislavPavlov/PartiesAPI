using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PartiesAPI.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

    }
}

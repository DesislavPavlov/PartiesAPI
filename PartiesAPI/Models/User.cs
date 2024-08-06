using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PartiesAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        public string? Email { get; set; }

        [JsonIgnore]
        public IEnumerable<Event>? Events { get; set; }

    }
}

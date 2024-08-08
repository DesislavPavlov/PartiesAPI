using System.ComponentModel.DataAnnotations;

namespace PartiesAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public ICollection<Event> OrganizedEvents { get; set; }

        public ICollection<Event> Events { get; set; }

    }
}

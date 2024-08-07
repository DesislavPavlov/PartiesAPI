using System.ComponentModel.DataAnnotations;

namespace PartiesAPI.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public DateTime StartTime { get; set;}

        [Required]
        public DateTime EndTime { get; set;}

        [Required]
        public string Organizer { get; set; }

        public ICollection<User>? Participants { get; set; }
    }
}

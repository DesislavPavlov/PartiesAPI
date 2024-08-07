using System.ComponentModel.DataAnnotations;

namespace PartiesAPI.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Location { get; set; }

        [Required]
        public DateTime StartTime { get; set;}

        [Required]
        public DateTime EndTime { get; set;}

        [Required]
        public int OrganizerId { get; set; }

        public ICollection<int>? ParticipantIds { get; set; }
    }
}

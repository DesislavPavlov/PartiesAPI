using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartiesAPI.Models
{
    public class EventParticipant
    {
        public int EventParticipantId { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public bool IsOrganizer { get; set; }
        public DateTime JoinDate { get; set; }
    }
}

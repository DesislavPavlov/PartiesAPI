using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartiesAPI.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set;}
        public DateTime EndDate { get; set;}
        public int OrganizerId { get; set; }
        public User Organizer { get; set; }
    }
}

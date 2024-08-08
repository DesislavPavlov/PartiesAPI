﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey("OrganizerId")]
        public User Organizer { get; set; }
    }
}

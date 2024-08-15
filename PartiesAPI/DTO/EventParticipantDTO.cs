namespace PartiesAPI.DTO
{
    public class EventParticipantDTO
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public DateTime JoinDate { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using PartiesAPI.Models;

namespace PartiesAPI.Data
{
    public class PartyDbContext : DbContext
    {
        public PartyDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventParticipant> EventParticipants { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var usersTable = modelBuilder.Entity<User>();
            usersTable.HasKey(u => u.Id);
            usersTable.Property(u => u.FirstName).IsRequired();
            usersTable.Property(u => u.LastName).IsRequired();
            usersTable.Property(u => u.Email).IsRequired();

            var eventsTable = modelBuilder.Entity<Event>();
            eventsTable.HasKey(e => e.Id);
            eventsTable.Property(e => e.Name).IsRequired();
            eventsTable.Property(e => e.Location).IsRequired();
            eventsTable.Property(e => e.StartDate).HasColumnType("datetime").IsRequired();
            eventsTable.Property(e => e.EndDate).HasColumnType("datetime").IsRequired();
            eventsTable.Property(e => e.Organizer).IsRequired();

            var eventParticipantsTable = modelBuilder.Entity<EventParticipant>();
            eventParticipantsTable.HasKey(ep => ep.Id);
            eventParticipantsTable.Property(ep => ep.UserId).IsRequired();
            eventParticipantsTable.Property(ep => ep.EventId).IsRequired();
            eventParticipantsTable.HasOne(ep => ep.Event).WithMany().HasForeignKey(ep => ep.EventId);
            eventParticipantsTable.HasOne(ep => ep.User).WithMany().HasForeignKey(ep => ep.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}

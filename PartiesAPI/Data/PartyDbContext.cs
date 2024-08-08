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
            usersTable.HasKey(u => u.UserId);
            usersTable.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            usersTable.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            usersTable.Property(u => u.Email).IsRequired();
            usersTable.HasIndex(u => u.Email).IsUnique();

            var eventsTable = modelBuilder.Entity<Event>();
            eventsTable.HasKey(e => e.EventId);
            eventsTable.Property(e => e.Name).HasMaxLength(100).IsRequired();
            eventsTable.Property(e => e.Location).HasMaxLength(100).IsRequired();
            eventsTable.Property(e => e.StartDate).HasColumnType("datetime").IsRequired();
            eventsTable.Property(e => e.EndDate).HasColumnType("datetime").IsRequired();
            eventsTable.Property(e => e.OrganizerId).IsRequired();
            eventsTable.HasOne(e => e.Organizer).WithOne().HasForeignKey<Event>(e => e.OrganizerId);

            var eventParticipantsTable = modelBuilder.Entity<EventParticipant>();
            eventParticipantsTable.HasKey(ep => ep.EventParticipantId);
            eventParticipantsTable.Property(ep => ep.UserId).IsRequired();
            eventParticipantsTable.Property(ep => ep.EventId).IsRequired();
            eventParticipantsTable.HasOne(ep => ep.User).WithMany().HasForeignKey(ep => ep.UserId);
            eventParticipantsTable.HasOne(ep => ep.Event).WithMany().HasForeignKey(ep => ep.EventId);

            base.OnModelCreating(modelBuilder);
        }
    }
}

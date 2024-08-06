using Microsoft.EntityFrameworkCore;
using PartiesAPI.Models;

namespace PartiesAPI.Data
{
    public class PartyDbContext : DbContext
    {
        public PartyDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var usersTable = modelBuilder.Entity<User>();
            usersTable.HasKey(u => u.Id);
            usersTable.Property(u => new { u.FirstName, u.LastName, u.Email }).IsRequired();

            var eventsTable = modelBuilder.Entity<Event>();
            eventsTable.HasKey(e => e.Id);
            eventsTable.Property(e => new {e.Name, e.Location, e.StartTime, e.EndTime, e.Organizer}).IsRequired();
            eventsTable.HasMany(e => e.Participants).WithMany(u => u.Events);
        }
    }
}

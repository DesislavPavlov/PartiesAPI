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
            usersTable.Property(u => u.FirstName).IsRequired();
            usersTable.Property(u => u.LastName).IsRequired();
            usersTable.Property(u => u.Email).IsRequired();

            var eventsTable = modelBuilder.Entity<Event>();
            eventsTable.HasKey(e => e.Id);
            eventsTable.Property(e => e.Name).IsRequired();
            eventsTable.Property(e => e.Location).IsRequired();
            eventsTable.Property(e => e.StartTime).HasColumnType("datetime").IsRequired();
            eventsTable.Property(e => e.EndTime).HasColumnType("datetime").IsRequired();
            eventsTable.Property(e => e.Organizer).IsRequired();
            eventsTable.HasMany(e => e.Participants);
        }
    }
}

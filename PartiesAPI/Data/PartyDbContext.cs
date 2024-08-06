using Microsoft.EntityFrameworkCore;
using PartiesAPI.Models;

namespace PartiesAPI.Data
{
    public class PartyDbContext : DbContext
    {
        public PartyDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
    }
}

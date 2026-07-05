using Microsoft.EntityFrameworkCore;
using Nozl.Model;
namespace Nozl

{
    public class NozlDbContext : DbContext
    {
        public NozlDbContext(DbContextOptions<NozlDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Room> Rooms { get; set; }
    }
}

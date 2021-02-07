using System.Data.Entity;
using GoogleCalenderDemo.Models;

namespace GoogleCalenderDemo
{
    public class CalendarContext : DbContext
    {
        public CalendarContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<GoogleRefreshToken> GoogleRefreshTokens { get; set; }
    }
}
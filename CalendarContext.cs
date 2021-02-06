using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
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

using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace GoogleCalenderDemo.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<GoogleCalenderDemo.CalendarContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }
    } 
}
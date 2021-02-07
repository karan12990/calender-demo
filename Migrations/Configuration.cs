using System.Data.Entity.Migrations;

namespace GoogleCalenderDemo.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<CalendarContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }
    }
}
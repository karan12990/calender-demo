using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GoogleCalenderDemo.Utils
{
    public class CalendarEvent
    {
        public string Id { get; set; }

        public string CalendarId { get; set; }

        [Required] public string Title { get; set; }

        [Required] public string Location { get; set; }

        [Required] public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<string> Attendees { get; set; }

        public int ColorId { get; set; }

        public string GuestEmailId { get; set; }
    }
}
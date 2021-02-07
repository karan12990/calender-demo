using System;
using System.ComponentModel.DataAnnotations;

namespace GoogleCalenderDemo.Dtos
{
    public class AvailabilityEvent
    {
        public DateTime StartDate { get; set; }
        public int Hour { get; set; }

        [Required] public string Title { get; set; }

        [Required] public string Location { get; set; }

        [Required] public string Description { get; set; }

        public string GuestEmailId { get; set; }
    }
}
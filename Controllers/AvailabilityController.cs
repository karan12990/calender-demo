using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using GoogleCalenderDemo.Dtos;
using GoogleCalenderDemo.Models;
using GoogleCalenderDemo.Utils;

namespace GoogleCalenderDemo.Controllers
{
    /// <summary>
    /// AvailabilityController : created for only testing ppurpose
    /// </summary>
    [Authorize]
    [Route("api/Availability")]
    public class AvailabilityController : ApiController
    {
        /// <summary>
        /// Post
        /// </summary>
        /// <param name="availabilityEvent"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Post(AvailabilityEvent availabilityEvent)
        {
            User userProfile = null;
            using (var context = new CalendarContext())
            {
                userProfile = context.Users.FirstOrDefault(c => c.UserName == User.Identity.Name);
            }

            var refreshToken = new CalendarContext().GoogleRefreshTokens
                .FirstOrDefault(c => c.UserName == User.Identity.Name)
                ?.RefreshToken;
            var authenticator = GoogleAuthorizationHelper.RefreshAuthenticator(refreshToken);

            var service = new GoogleCalendarServiceProxy(authenticator);


            var model = service.GetEvents(userProfile.Email, availabilityEvent.StartDate);
            model = model
                .Where(x => x.StartDate >= availabilityEvent.StartDate && x.StartDate <= availabilityEvent.StartDate)
                .OrderBy(y => y.StartDate).ToList();
            //var availability = 3;
            var index = 0;
            var startTime = DateTime.Today;
            DateTime? endTime = null;
            var diff = 0;
            //var model1 = new List<CalendarEvent>();

            //model1.Add(new CalendarEvent
            //{
            //    Title = "Kashin standup meeting",
            //    StartDate = DateTime.Parse("2021-10-07 12:00:00"),
            //    EndDate = DateTime.Parse("2021-10-07 12:30:00"),

            //});

            //model1.Add(new CalendarEvent
            //{
            //    Title = "Kashin standup meeting1",
            //    StartDate = DateTime.Parse("2021-10-07 16:00:00"),
            //    EndDate = DateTime.Parse("2021-10-07 16:10:00"),

            //});

            //model1.Add(new CalendarEvent
            //{
            //    Title = "Kashin standup meeting2",
            //    StartDate = DateTime.Parse("2021-10-07 22:00:00"),
            //    EndDate = DateTime.Parse("2021-10-07 23:30:00"),

            //});

            //model1.Add(new CalendarEvent
            //{
            //    Title = "Kashin standup meeting3",
            //    StartDate = DateTime.Parse("2021-10-07 11:00:00"),
            //    EndDate = DateTime.Parse("2021-10-07 12:00:00"),

            //});

            foreach (var item in model)
            {
                if (index != 0)
                {
                    //item.EndDate = model
                }

                diff = Math.Abs((item.StartDate - startTime).Hours);

                if (diff >= availabilityEvent.Hour)
                {
                    // available
                    // starttime + 3
                    endTime = startTime.AddHours(3);

                    var calendarEvent = new CalendarEvent();
                    calendarEvent.Description = availabilityEvent.Description;
                    calendarEvent.Title = availabilityEvent.Title;
                    calendarEvent.Location = availabilityEvent.Location;
                    calendarEvent.Description = availabilityEvent.Description;
                    calendarEvent.StartDate = startTime;
                    calendarEvent.EndDate = (DateTime) endTime;
                    calendarEvent.Attendees = new List<string>
                    {
                        availabilityEvent.GuestEmailId
                    };

                    calendarEvent.CalendarId = userProfile.Email;
                    service.CreateEvent(calendarEvent);

                    break;
                }

                startTime = item.EndDate;

                index++;
            }

            if (endTime == null) endTime = startTime.AddHours(3);

            return Json(new {data = model});
        }
    }
}
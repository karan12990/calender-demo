using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GoogleCalenderDemo.Dtos;
using GoogleCalenderDemo.Models;
using GoogleCalenderDemo.Utils;

namespace GoogleCalenderDemo.Controllers
{
    /// <summary>
    /// EventController
    /// </summary>
    [Authorize]
    public class EventController : Controller
    {
        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // By default, we display all the events from the last 10 days
            return ListEvents(DateTime.Today, DateTime.Today);
        }

        /// <summary>
        /// Get events by date 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ActionResult ListEvents(DateTime startDate, DateTime endDate)
        {
            User userProfile = null;
            using (var context = new CalendarContext())
            {
                userProfile = context.Users.FirstOrDefault(c => c.UserName == User.Identity.Name);
            }

            if (userProfile == null) return RedirectToAction("Register", "Account");

            var authenticator = GetAuthenticator();

            var service = new GoogleCalendarServiceProxy(authenticator);
            var model = service.GetEvents(userProfile.Email, startDate, endDate.AddHours(24));

            ViewBag.StartDate = startDate.ToShortDateString();
            ViewBag.EndDate = endDate.ToShortDateString();
            return View("Index", model);
        }

        /// <summary>
        /// GET: /CalendarEvent/Crate
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Create()
        {
            var calendarId = string.Empty;
            using (var context = new CalendarContext())
            {
                calendarId = context.Users.FirstOrDefault(c => c.UserName == User.Identity.Name).Email;
            }

            var model = new CalendarEvent
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMinutes(60)
            };
            return View(model);
        }

        /// <summary>
        /// GET : Availability
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Availability()
        {
            var model = new AvailabilityEvent
            {
                StartDate = DateTime.Now,
                Hour = 1
            };
            return View(model);
        }


        /// <summary>
        /// POST : Availability
        /// </summary>
        /// <param name="availabilityEvent"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Availability(AvailabilityEvent availabilityEvent)
        {
            if (!ModelState.IsValid) return View("Availability", "Event");
            User userProfile = null;
            using (var context = new CalendarContext())
            {
                userProfile = context.Users.FirstOrDefault(c => c.UserName == User.Identity.Name);
            }

            var authenticator = GetAuthenticator();
            var service = new GoogleCalendarServiceProxy(authenticator);

            var model = service.GetEvents(userProfile.Email, availabilityEvent.StartDate);
            model = model.OrderBy(y => y.StartDate).ToList();
            //var availability = 3;
            var index = 0;
            var startTime = availabilityEvent.StartDate.Date;
            DateTime? endTime = null;
            var diff = 0;

            var calendarEvent = new CalendarEvent();
            calendarEvent.Description = availabilityEvent.Description;
            calendarEvent.Title = availabilityEvent.Title;
            calendarEvent.Location = availabilityEvent.Location;
            calendarEvent.Description = availabilityEvent.Description;
            calendarEvent.Attendees = new List<string>
            {
                availabilityEvent.GuestEmailId
            };

            calendarEvent.CalendarId = userProfile.Email;

            var isError = true;
            foreach (var item in model)
            {
                diff = Math.Abs((item.StartDate - startTime).Hours);

                if (diff >= availabilityEvent.Hour)
                {
                    // available
                    // starttime + availability
                    endTime = startTime.AddHours(availabilityEvent.Hour);

                    calendarEvent.StartDate = startTime;
                    calendarEvent.EndDate = (DateTime) endTime;

                    service.CreateEvent(calendarEvent);

                    isError = false;
                    break;
                }

                startTime = item.EndDate;
            }

            if (endTime == null)
            {
                endTime = startTime.AddHours(availabilityEvent.Hour);

                // create event

                calendarEvent.StartDate = startTime;
                calendarEvent.EndDate = (DateTime) endTime;

                service.CreateEvent(calendarEvent);

                isError = false;
            }

            if (isError)
                ViewBag.error = "We have not found availability. Please check availability and create the event";
            else
                ViewBag.success =
                    $"Successfully created event : start {startTime.ToString("g")} end {endTime.Value.ToString("g")}";

            return View();

        }


        /// <summary>
        /// POST: /CalendarEvent/Create
        /// </summary>
        /// <param name="calendarEvent"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(CalendarEvent calendarEvent)
        {
            if (!ModelState.IsValid) return RedirectToAction("Index", "Event");
            var authenticator = GetAuthenticator();
            var service = new GoogleCalendarServiceProxy(authenticator);

            calendarEvent.Attendees = new List<string>
            {
                calendarEvent.GuestEmailId
            };

            calendarEvent.CalendarId = "khatri.karankhatri.karan@gmail.com";
            service.CreateEvent(calendarEvent);

            return RedirectToAction("Index", "Event");
        }

        private GoogleAuthenticator GetAuthenticator()
        {
            var authenticator = (GoogleAuthenticator) Session["authenticator"];

            if (authenticator != null && authenticator.IsValid) return authenticator;
            // Get a new Authenticator using the Refresh Token
            var refreshToken = new CalendarContext().GoogleRefreshTokens
                .FirstOrDefault(c => c.UserName == User.Identity.Name)
                ?.RefreshToken;
            authenticator = GoogleAuthorizationHelper.RefreshAuthenticator(refreshToken);
            Session["authenticator"] = authenticator;

            return authenticator;
        }
    }
}
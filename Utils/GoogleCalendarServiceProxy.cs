using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Authentication;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

namespace GoogleCalenderDemo.Utils
{
    public class GoogleCalendarServiceProxy
    {
        private readonly IAuthenticator _authenticator;

        public GoogleCalendarServiceProxy(GoogleAuthenticator googleAuthenticator)
        {
            _authenticator = googleAuthenticator.Authenticator;
        }

        public IEnumerable<Calendar> GetCalendars()
        {
            var calendarService = new CalendarService(_authenticator);
            var calendars = calendarService.CalendarList.List().Fetch().Items.Select(c => new Calendar
            {
                Id = c.Id,
                Title = c.Summary,
                Location = c.Location,
                Description = c.Description
            });

            return calendars;
        }

        public Calendar GetCalendar(string calendarId)
        {
            var calendarService = new CalendarService(_authenticator);
            var calendar = calendarService.CalendarList.List().Fetch().Items
                .FirstOrDefault(c => c.Summary.Contains(calendarId));
            if (calendar == null) throw new GoogleCalendarServiceProxyException("There's no calendar with that id");

            return new Calendar
            {
                Id = calendar.Id,
                Title = calendar.Summary,
                Location = calendar.Location,
                Description = calendar.Description
            };
        }


        public IEnumerable<CalendarEvent> GetEvents(string calendarId, DateTime startDate, DateTime endDate)
        {
            List<CalendarEvent> calendarEvents = null;
            var calendarService = new CalendarService(_authenticator);
            var calendar = GetCalendar(calendarId);

            var request = calendarService.Events.List(calendar.Id);
            request.TimeMin = startDate.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fffK");
            request.TimeMax = endDate.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fffK");
            var result = request.Fetch().Items;

            if (result != null)
                calendarEvents = result.Select(c => new CalendarEvent
                {
                    Id = c.Id,
                    CalendarId = calendarId,
                    Title = c.Summary,
                    Location = c.Location,
                    StartDate = DateTime.Parse(c.Start.DateTime),
                    EndDate = DateTime.Parse(c.End.DateTime),
                    Description = c.Description,
                    ColorId = c.ColorId == null ? 0 : int.Parse(c.ColorId),
                    Attendees = c.Attendees != null ? c.Attendees.Select(attendee => attendee.Email) : null
                }).ToList();

            return calendarEvents;
        }

        public IEnumerable<CalendarEvent> GetEvents(string calendarId, DateTime startDate)
        {
            List<CalendarEvent> calendarEvents = null;
            var calendarService = new CalendarService(_authenticator);
            var calendar = GetCalendar(calendarId);

            var request = calendarService.Events.List(calendar.Id);
            request.TimeMin = startDate.Date.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fffK");
            request.TimeMax = startDate.Date.AddHours(24).ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fffK");

            var result = request.Fetch().Items;

            if (result != null)
                calendarEvents = result.Select(c => new CalendarEvent
                {
                    Id = c.Id,
                    CalendarId = calendarId,
                    Title = c.Summary,
                    Location = c.Location,
                    StartDate = DateTime.Parse(c.Start.DateTime),
                    EndDate = DateTime.Parse(c.End.DateTime),
                    Description = c.Description,
                    ColorId = c.ColorId == null ? 0 : int.Parse(c.ColorId),
                    Attendees = c.Attendees != null ? c.Attendees.Select(attendee => attendee.Email) : null
                }).ToList();

            return calendarEvents;
        }

        public CalendarEvent GetEvent(string calendarId, string eventId)
        {
            var calendarService = new CalendarService(_authenticator);
            var calendarEvent = calendarService.Events.Get(calendarId, eventId).Fetch();
            if (calendarEvent == null)
                throw new GoogleCalendarServiceProxyException("There is no event stored in the calendar with that id");

            return new CalendarEvent
            {
                Id = calendarEvent.Id,
                CalendarId = calendarId,
                Title = calendarEvent.Summary,
                Location = calendarEvent.Location,
                StartDate = DateTime.Parse(calendarEvent.Start.DateTime),
                EndDate = DateTime.Parse(calendarEvent.End.DateTime),
                Description = calendarEvent.Description,
                ColorId = int.Parse(calendarEvent.ColorId),
                Attendees = calendarEvent.Attendees != null ? calendarEvent.Attendees.Select(c => c.Email) : null
            };
        }

        public bool CreateEvent(CalendarEvent calendarEvent)
        {
            var calendarService = new CalendarService(_authenticator);
            var calendar = GetCalendar(calendarEvent.CalendarId);

            var newEvent = new Event
            {
                Summary = calendarEvent.Title,
                Location = calendarEvent.Location,
                Description = calendarEvent.Description,
                Start = new EventDateTime
                    {DateTime = calendarEvent.StartDate.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fffK")},
                End = new EventDateTime
                    {DateTime = calendarEvent.EndDate.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fffK")},
                Attendees = calendarEvent.Attendees != null
                    ? calendarEvent.Attendees.Select(email => new EventAttendee {Email = email}).ToList()
                    : null
            };

            var result = calendarService.Events.Insert(newEvent, calendar.Id).Fetch();

            return result != null;
        }
    }

    public class GoogleCalendarServiceProxyException : Exception
    {
        public GoogleCalendarServiceProxyException(string errorMessage) : base(errorMessage)
        {
        }
    }
}
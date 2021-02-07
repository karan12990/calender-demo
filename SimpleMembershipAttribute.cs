using System;
using System.Threading;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace GoogleCalenderDemo
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class SimpleMembershipAttribute : ActionFilterAttribute
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Ensure ASP.NET Simple Membership is initialized only once per app start
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                try
                {
                    using (var context = new CalendarContext())
                    {
                        context.Database.CreateIfNotExists();
                        context.Database.Initialize(true);
                    }

                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "User", "UserId", "UserName", true);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        "The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588",
                        ex);
                }
            }
        }
    }
}
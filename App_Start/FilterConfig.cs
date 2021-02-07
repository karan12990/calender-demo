﻿using System.Web.Mvc;

namespace GoogleCalenderDemo
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new SimpleMembershipAttribute());
        }
    }
}
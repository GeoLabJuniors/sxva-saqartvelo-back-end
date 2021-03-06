﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using sxva_saqartvelo_back_end.Models;

namespace sxva_saqartvelo_back_end.Helpers
{
    public class LoginHelper
    {
        public static void LoggedInUser(Freelancer freelancer)
        {
            HttpContext.Current.Session["freelancer"] = freelancer;
            isLoggedIn = true;
        }

        public static void Logout()
        {
            HttpContext.Current.Session["freelancer"] = null;
            isLoggedIn = false;
        }

        public static bool isLoggedIn;

        public static Freelancer freelancer()
        {
            return (Freelancer)HttpContext.Current.Session["freelancer"];
        }
    }
}
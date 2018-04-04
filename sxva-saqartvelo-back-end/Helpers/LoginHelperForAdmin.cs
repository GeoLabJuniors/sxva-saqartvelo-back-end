﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using sxva_saqartvelo_back_end.Models;

namespace sxva_saqartvelo_back_end.Helpers
{
    public class LoginHelperForAdmin
    {
        public static void LoggedInAdmin(Admin admin)
        {
            HttpContext.Current.Session["admin"] = admin;
            isLoggedIn = true;
        }

        public static void Logout()
        {
            HttpContext.Current.Session["admin"] = null;
            isLoggedIn = false;
        }

        public static bool isLoggedIn;

        public static Admin admin()
        {
            return (Admin)HttpContext.Current.Session["admin"];
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using sxva_saqartvelo_back_end.Models;

namespace sxva_saqartvelo_back_end.Helpers
{
    public class LoginHelperForCompany
    {
        public static void LoggedInCompany(Company company)
        {
            HttpContext.Current.Session["company"] = company;
            isLoggedIn = true;
        }

        public static void Logout()
        {
            HttpContext.Current.Session["company"] = null;
            isLoggedIn = false;
        }

        public static bool isLoggedIn;

        public static Company company()
        {
            return (Company)HttpContext.Current.Session["company"];
        }
    }
}
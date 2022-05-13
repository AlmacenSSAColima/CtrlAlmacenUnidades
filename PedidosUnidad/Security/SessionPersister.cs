using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PedidosUnidad.Security
{
    public static class SessionPersister
    {
        static string usernameSessionvar = "CurrentUser";
        public static CurrentUser CurrentUser
        {
            get
            {
                if (HttpContext.Current == null)
                    return new CurrentUser();
                var sessionVar = HttpContext.Current.Session[usernameSessionvar];
                if (sessionVar != null)
                    return sessionVar as CurrentUser;
                return null;
            }
            set
            {
                HttpContext.Current.Session[usernameSessionvar] = value;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PedidosUnidad.Security
{
    public class CustomAuthorizeAtribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (SessionPersister.CurrentUser == null)
            {

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
            }
            else
            {
                if (!SessionPersister.CurrentUser.login)
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
                else
                {
                    if (!string.IsNullOrEmpty(Roles))
                    {
                        AccontModelProcess am = new AccontModelProcess();
                        CustomPrincipal mp = new CustomPrincipal(am.find(SessionPersister.CurrentUser.name_user));
                        if (!mp.IsInRole(Roles))
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "AccesDenied", action = "Index" }));

                    }

                }
            }
        }
    }

}
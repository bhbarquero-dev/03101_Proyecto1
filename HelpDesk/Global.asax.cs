using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using HelpDesk.Seguridad;

namespace HelpDesk
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        // Pasa el rol guardado en la cookie al usuario de la solicitud, para que funcione [Authorize(Roles = ...)]
        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            var sesion = SesionUsuario.Obtener(Context.User);
            if (sesion != null)
            {
                Context.User = new GenericPrincipal(Context.User.Identity, new[] { sesion.Rol });
            }
        }
    }
}

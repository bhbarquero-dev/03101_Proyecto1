using System.Web.Mvc;
using HelpDesk.Models;

namespace HelpDesk.Seguridad
{
    // Restringe una accion al rol Administrador.
    // A un usuario autenticado sin el rol le responde 403 en lugar de mandarlo al login,
    // que es lo que hace AuthorizeAttribute por defecto.
    public class AutorizarAdministradorAttribute : AuthorizeAttribute
    {
        public AutorizarAdministradorAttribute()
        {
            Roles = Usuario.RolAdministrador;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
                return;
            }

            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.StatusCode = 403;
                filterContext.Result = new JsonResult { Data = new { error = "No tiene permiso para realizar esta acción." } };
            }
            else
            {
                filterContext.Result = new HttpStatusCodeResult(403);
            }
        }
    }
}

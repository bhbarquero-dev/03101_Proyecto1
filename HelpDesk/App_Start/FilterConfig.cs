using System.Web;
using System.Web.Mvc;

namespace HelpDesk
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            // Denegar por defecto: toda accion exige sesion salvo las marcadas con [AllowAnonymous]
            filters.Add(new AuthorizeAttribute());
        }
    }
}

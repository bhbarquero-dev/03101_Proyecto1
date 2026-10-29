using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using HelpDesk.Datos;
using HelpDesk.Seguridad;

namespace HelpDesk.Controllers
{
    public class TicketsController : Controller
    {
        // GET: /Tickets/MisTickets (HU-02)
        public ActionResult MisTickets()
        {
            var sesion = SesionUsuario.Obtener(User);

            using (var db = new HelpDeskContext())
            {
                // Solo los tickets que solicito el usuario de la sesion, del mas reciente al mas antiguo
                var tickets = db.Tickets
                    .AsNoTracking()
                    .Include(t => t.Estado)
                    .Include(t => t.Asignado)
                    .Where(t => t.IdUsuarioSolicitante == sesion.Id)
                    .OrderByDescending(t => t.FechaCreacion)
                    .ToList();

                return View(tickets);
            }
        }
    }
}

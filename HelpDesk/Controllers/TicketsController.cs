using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using HelpDesk.Datos;
using HelpDesk.Models;
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

        // GET: /Tickets/Crear (HU-01)
        public ActionResult Crear()
        {
            return View(new CrearTicketViewModel());
        }

        // POST: /Tickets/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(CrearTicketViewModel modelo)
        {
            if (!string.IsNullOrEmpty(modelo.Categoria) && !CrearTicketViewModel.Categorias.Contains(modelo.Categoria))
            {
                ModelState.AddModelError("Categoria", "Seleccione una categoría válida.");
            }
            if (!ModelState.IsValid) return View(modelo);

            // El solicitante sale de la sesion; el estado inicial y las fechas los asigna el constructor
            var ticket = new Ticket
            {
                Titulo = modelo.Titulo.Trim(),
                Descripcion = modelo.Descripcion.Trim(),
                Categoria = modelo.Categoria,
                IdUsuarioSolicitante = SesionUsuario.Obtener(User).Id
            };

            using (var db = new HelpDeskContext())
            {
                db.Tickets.Add(ticket);
                db.SaveChanges();
            }

            // Redirige para que recargar la pagina no registre el ticket otra vez
            TempData["Mensaje"] = "El ticket N.º " + ticket.Id + " se registró correctamente.";
            return RedirectToAction("MisTickets");
        }
    }
}

using System;
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
        private const string TicketNoEncontrado = "El ticket no existe o no tiene acceso a él.";

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

        // GET: /Tickets/Todos (solo Administrador)
        public ActionResult Todos()
        {
            if (!SesionUsuario.Obtener(User).EsAdministrador) return new HttpStatusCodeResult(403);

            using (var db = new HelpDeskContext())
            {
                var tickets = db.Tickets
                    .AsNoTracking()
                    .Include(t => t.Estado)
                    .Include(t => t.Solicitante)
                    .Include(t => t.Asignado)
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

        // GET: /Tickets/Detalle/5
        public ActionResult Detalle(int id = 0)
        {
            using (var db = new HelpDeskContext())
            {
                var ticket = TicketsVisibles(db)
                    .AsNoTracking()
                    .Include(t => t.Estado)
                    .Include(t => t.Solicitante)
                    .Include(t => t.Asignado)
                    .SingleOrDefault(t => t.Id == id);

                if (ticket == null) return HttpNotFound();
                return View(ticket);
            }
        }

        // GET: /Tickets/Comentarios/5 (AJAX)
        public ActionResult Comentarios(int id = 0)
        {
            using (var db = new HelpDeskContext())
            {
                if (!TicketsVisibles(db).Any(t => t.Id == id)) return ErrorJson(404, TicketNoEncontrado);

                var comentarios = db.Comentarios
                    .Where(c => c.IdTicket == id)
                    .OrderBy(c => c.Fecha).ThenBy(c => c.Id)
                    .Select(c => new { Autor = c.Usuario.Nombre, c.Fecha, c.Texto })
                    .ToList()
                    .Select(c => new { autor = c.Autor, fecha = FormatoFecha(c.Fecha), texto = c.Texto });

                return Json(new { comentarios }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: /Tickets/AgregarComentario/5 (AJAX, HU-03)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarComentario(ComentarioViewModel modelo, int id = 0)
        {
            var sesion = SesionUsuario.Obtener(User);

            using (var db = new HelpDeskContext())
            {
                var ticket = TicketsVisibles(db).SingleOrDefault(t => t.Id == id);
                if (ticket == null) return ErrorJson(404, TicketNoEncontrado);

                if (!ModelState.IsValid)
                {
                    string error = ModelState.Values.SelectMany(v => v.Errors).First().ErrorMessage;
                    return ErrorJson(400, error);
                }

                var comentario = new Comentario
                {
                    IdTicket = ticket.Id,
                    IdUsuario = sesion.Id,
                    Texto = modelo.Texto.Trim()
                };
                db.Comentarios.Add(comentario);
                ticket.FechaActualizacion = comentario.Fecha;
                db.SaveChanges();

                return Json(new
                {
                    autor = sesion.Nombre,
                    fecha = FormatoFecha(comentario.Fecha),
                    texto = comentario.Texto
                });
            }
        }

        // Un Usuario solo ve los tickets que solicito; un Administrador ve todos.
        // Un ticket ajeno se trata igual que uno inexistente, para no revelar que existe.
        private IQueryable<Ticket> TicketsVisibles(HelpDeskContext db)
        {
            var sesion = SesionUsuario.Obtener(User);
            if (sesion.EsAdministrador) return db.Tickets;

            int idUsuario = sesion.Id;
            return db.Tickets.Where(t => t.IdUsuarioSolicitante == idUsuario);
        }

        private JsonResult ErrorJson(int estado, string mensaje)
        {
            Response.StatusCode = estado;
            Response.TrySkipIisCustomErrors = true;
            return Json(new { error = mensaje }, JsonRequestBehavior.AllowGet);
        }

        private static string FormatoFecha(DateTime fecha)
        {
            return fecha.ToString("dd/MM/yyyy HH:mm");
        }
    }
}

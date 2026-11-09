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
        private const int LargoMaximoBusqueda = 150;

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

                ViewBag.Estados = ListaEstados(db);
                return View(tickets);
            }
        }

        // GET: /Tickets/Todos (solo Administrador)
        [AutorizarAdministrador]
        public ActionResult Todos()
        {
            using (var db = new HelpDeskContext())
            {
                var tickets = db.Tickets
                    .AsNoTracking()
                    .Include(t => t.Estado)
                    .Include(t => t.Solicitante)
                    .Include(t => t.Asignado)
                    .OrderByDescending(t => t.FechaCreacion)
                    .ToList();

                ViewBag.Estados = ListaEstados(db);
                return View(tickets);
            }
        }

        // GET: /Tickets/Buscar?texto=...&idEstado=... (AJAX, HU-07)
        // El texto se compara con el numero, el titulo y el nombre del estado.
        // Un Usuario solo encuentra sus propios tickets; un Administrador, todos.
        public ActionResult Buscar(BusquedaTicketsViewModel criterios)
        {
            int idEstado = criterios.IdEstado;
            string texto = (criterios.Texto ?? "").Trim();
            if (texto.Length > LargoMaximoBusqueda) texto = texto.Substring(0, LargoMaximoBusqueda);

            using (var db = new HelpDeskContext())
            {
                var consulta = TicketsVisibles(db).AsNoTracking();

                if (idEstado != 0) consulta = consulta.Where(t => t.IdEstado == idEstado);

                if (texto.Length > 0)
                {
                    // Acepta el numero con o sin el prefijo "#"
                    int numero;
                    bool esNumero = int.TryParse(texto.TrimStart('#'), out numero);
                    consulta = consulta.Where(t => (esNumero && t.Id == numero)
                        || t.Titulo.Contains(texto)
                        || t.Estado.Nombre.Contains(texto));
                }

                var tickets = consulta
                    .OrderByDescending(t => t.FechaCreacion)
                    .Select(t => new
                    {
                        t.Id,
                        t.Titulo,
                        Solicitante = t.Solicitante.Nombre,
                        t.Categoria,
                        t.IdEstado,
                        Estado = t.Estado.Nombre,
                        Asignado = t.Asignado.Nombre,
                        t.FechaCreacion,
                        t.FechaActualizacion
                    })
                    .ToList()
                    .Select(t => new
                    {
                        id = t.Id,
                        titulo = t.Titulo,
                        solicitante = t.Solicitante,
                        categoria = t.Categoria,
                        estado = t.Estado,
                        clase = EstadoTicket.ClaseBadge(t.IdEstado),
                        asignado = t.Asignado,
                        creado = FormatoFecha(t.FechaCreacion),
                        actualizado = FormatoFecha(t.FechaActualizacion)
                    });

                return Json(new { tickets }, JsonRequestBehavior.AllowGet);
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

                // Solo el Administrador ve los controles para cambiar el estado y asignar el ticket
                if (SesionUsuario.Obtener(User).EsAdministrador)
                {
                    ViewBag.Estados = new SelectList(db.EstadosTicket.OrderBy(e => e.Id).ToList(), "Id", "Nombre", ticket.IdEstado);
                    ViewBag.Administradores = new SelectList(AdministradoresActivos(db).OrderBy(u => u.Nombre).ToList(), "Id", "Nombre", ticket.IdUsuarioAsignado);
                }
                return View(ticket);
            }
        }

        // POST: /Tickets/CambiarEstado/5 (AJAX, HU-05)
        [HttpPost]
        [AutorizarAdministrador]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarEstado(int id = 0, int idEstado = 0)
        {
            using (var db = new HelpDeskContext())
            {
                var ticket = db.Tickets.SingleOrDefault(t => t.Id == id);
                if (ticket == null) return ErrorJson(404, TicketNoEncontrado);

                var estado = db.EstadosTicket.SingleOrDefault(e => e.Id == idEstado);
                if (estado == null) return ErrorJson(400, "El estado seleccionado no existe.");

                // Elegir el mismo estado no es un cambio: no se guarda ni se actualiza la fecha
                bool cambio = ticket.IdEstado != estado.Id;
                if (cambio)
                {
                    ticket.IdEstado = estado.Id;
                    ticket.FechaActualizacion = DateTime.Now;
                    db.SaveChanges();
                }

                return Json(new
                {
                    cambio,
                    estado = estado.Nombre,
                    clase = EstadoTicket.ClaseBadge(estado.Id),
                    fecha = FormatoFecha(ticket.FechaActualizacion)
                });
            }
        }

        // POST: /Tickets/Asignar/5 (AJAX, HU-04)
        [HttpPost]
        [AutorizarAdministrador]
        [ValidateAntiForgeryToken]
        public ActionResult Asignar(int id = 0, int idUsuario = 0)
        {
            using (var db = new HelpDeskContext())
            {
                var ticket = db.Tickets.SingleOrDefault(t => t.Id == id);
                if (ticket == null) return ErrorJson(404, TicketNoEncontrado);

                // Solo se puede asignar a un Administrador activo
                var administrador = AdministradoresActivos(db).SingleOrDefault(u => u.Id == idUsuario);
                if (administrador == null) return ErrorJson(400, "Seleccione un administrador activo.");

                // Asignar al mismo responsable no es un cambio: no se guarda ni se actualiza la fecha
                bool cambio = ticket.IdUsuarioAsignado != administrador.Id;
                if (cambio)
                {
                    ticket.IdUsuarioAsignado = administrador.Id;
                    ticket.FechaActualizacion = DateTime.Now;
                    db.SaveChanges();
                }

                return Json(new
                {
                    cambio,
                    asignado = administrador.Nombre,
                    fecha = FormatoFecha(ticket.FechaActualizacion)
                });
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

        private static SelectList ListaEstados(HelpDeskContext db)
        {
            return new SelectList(db.EstadosTicket.OrderBy(e => e.Id).ToList(), "Id", "Nombre");
        }

        private static IQueryable<Usuario> AdministradoresActivos(HelpDeskContext db)
        {
            return db.Usuarios.Where(u => u.Rol == Usuario.RolAdministrador && u.Estado);
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

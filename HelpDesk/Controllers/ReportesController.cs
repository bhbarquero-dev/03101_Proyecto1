using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HelpDesk.Datos;
using HelpDesk.Models;
using HelpDesk.Seguridad;

namespace HelpDesk.Controllers
{
    // Reportes basicos (HU-06), solo para el Administrador
    [AutorizarAdministrador]
    public class ReportesController : Controller
    {
        private const int DiasPorDefecto = 90;

        // GET: /Reportes?desde=2026-08-01&hasta=2026-10-31
        public ActionResult Index(DateTime? desde, DateTime? hasta)
        {
            // Una fecha con formato invalido deja el valor en null y se usa el periodo por defecto
            ModelState.Clear();

            var hoy = DateTime.Today;
            var modelo = new ReporteViewModel
            {
                Desde = (desde ?? hoy.AddDays(-DiasPorDefecto)).Date,
                Hasta = (hasta ?? hoy).Date
            };

            if (modelo.Desde > modelo.Hasta)
            {
                ModelState.AddModelError("", "La fecha «Desde» no puede ser posterior a la fecha «Hasta».");
                return View(modelo);
            }

            DateTime limite = modelo.Hasta.AddDays(1);

            using (var db = new HelpDeskContext())
            {
                var estados = db.EstadosTicket.OrderBy(e => e.Id).ToList();

                var tickets = db.Tickets
                    .AsNoTracking()
                    .Where(t => t.FechaCreacion >= modelo.Desde && t.FechaCreacion < limite)
                    .Select(t => new DatosTicket { IdEstado = t.IdEstado, FechaCreacion = t.FechaCreacion, FechaActualizacion = t.FechaActualizacion })
                    .ToList();

                modelo.Total = tickets.Count;
                modelo.Resueltos = tickets.Count(t => ReporteViewModel.EstaResuelto(t.IdEstado));
                modelo.PromedioResolucion = PromedioResolucion(tickets);

                // Todos los estados, aunque no tengan tickets en el periodo
                modelo.PorEstado = estados
                    .Select(e => new FilaReporteEstado
                    {
                        IdEstado = e.Id,
                        Estado = e.Nombre,
                        Cantidad = tickets.Count(t => t.IdEstado == e.Id)
                    })
                    .ToList();

                // Un renglon por cada mes del periodo, aunque no tenga tickets
                for (var mes = new DateTime(modelo.Desde.Year, modelo.Desde.Month, 1); mes < limite; mes = mes.AddMonths(1))
                {
                    var delMes = tickets.Where(t => t.FechaCreacion.Year == mes.Year && t.FechaCreacion.Month == mes.Month).ToList();
                    modelo.PorMes.Add(new FilaReportePeriodo
                    {
                        Periodo = NombreMes(mes),
                        Creados = delMes.Count,
                        Resueltos = delMes.Count(t => ReporteViewModel.EstaResuelto(t.IdEstado)),
                        PromedioResolucion = PromedioResolucion(delMes)
                    });
                }
            }

            return View(modelo);
        }

        private static TimeSpan? PromedioResolucion(IEnumerable<DatosTicket> tickets)
        {
            var resueltos = tickets.Where(t => ReporteViewModel.EstaResuelto(t.IdEstado)).ToList();
            if (resueltos.Count == 0) return null;
            return TimeSpan.FromTicks((long)resueltos.Average(t => (t.FechaActualizacion - t.FechaCreacion).Ticks));
        }

        private static string NombreMes(DateTime mes)
        {
            string nombre = ReporteViewModel.Cultura.DateTimeFormat.GetMonthName(mes.Month);
            return char.ToUpper(nombre[0], ReporteViewModel.Cultura) + nombre.Substring(1) + " " + mes.Year;
        }

        private class DatosTicket
        {
            public int IdEstado { get; set; }
            public DateTime FechaCreacion { get; set; }
            public DateTime FechaActualizacion { get; set; }
        }
    }
}

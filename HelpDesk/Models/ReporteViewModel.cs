using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace HelpDesk.Models
{
    // Reporte de tickets por estado y por periodo (HU-06)
    public class ReporteViewModel
    {
        public static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("es-CR");

        public ReporteViewModel()
        {
            PorEstado = new List<FilaReporteEstado>();
            PorMes = new List<FilaReportePeriodo>();
        }

        // Rango de fechas de creacion de los tickets incluidos (ambos dias inclusive)
        [Display(Name = "Desde"), DataType(DataType.Date)]
        public DateTime Desde { get; set; }

        [Display(Name = "Hasta"), DataType(DataType.Date)]
        public DateTime Hasta { get; set; }

        public int Total { get; set; }

        // Tickets en estado Resuelto o Cerrado
        public int Resueltos { get; set; }

        public TimeSpan? PromedioResolucion { get; set; }

        public List<FilaReporteEstado> PorEstado { get; set; }

        public List<FilaReportePeriodo> PorMes { get; set; }

        // El modelo minimo no guarda la fecha de cierre: el tiempo de resolucion se aproxima
        // con FechaActualizacion - FechaCreacion de los tickets resueltos o cerrados
        public static bool EstaResuelto(int idEstado)
        {
            return idEstado == EstadoTicket.Resuelto || idEstado == EstadoTicket.Cerrado;
        }

        public static string FormatoDuracion(TimeSpan? duracion)
        {
            if (duracion == null) return "—";
            var d = duracion.Value;
            return d.TotalHours < 24
                ? d.TotalHours.ToString("0.0", Cultura) + " h"
                : d.TotalDays.ToString("0.0", Cultura) + " días";
        }

        public string Porcentaje(int cantidad)
        {
            return Total == 0 ? "0 %" : ((double)cantidad / Total).ToString("0 %", Cultura);
        }
    }

    public class FilaReporteEstado
    {
        public int IdEstado { get; set; }
        public string Estado { get; set; }
        public int Cantidad { get; set; }
    }

    public class FilaReportePeriodo
    {
        public string Periodo { get; set; }
        public int Creados { get; set; }
        public int Resueltos { get; set; }
        public TimeSpan? PromedioResolucion { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.Models
{
    // Tabla Tickets (Database/03_Tickets.sql)
    [Table("Tickets")]
    public class Ticket
    {
        public Ticket()
        {
            // EF no aplica los DEFAULT de la base de datos, por eso se asignan aqui
            IdEstado = EstadoTicket.Abierto;
            FechaCreacion = DateTime.Now;
            FechaActualizacion = FechaCreacion;
            Comentarios = new HashSet<Comentario>();
        }

        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Titulo { get; set; }

        [Required]
        public string Descripcion { get; set; }

        public int IdUsuarioSolicitante { get; set; }

        // null mientras no se asigna un responsable
        public int? IdUsuarioAsignado { get; set; }

        public int IdEstado { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaActualizacion { get; set; }

        [Required, StringLength(50)]
        public string Categoria { get; set; }

        [ForeignKey("IdUsuarioSolicitante")]
        public virtual Usuario Solicitante { get; set; }

        [ForeignKey("IdUsuarioAsignado")]
        public virtual Usuario Asignado { get; set; }

        [ForeignKey("IdEstado")]
        public virtual EstadoTicket Estado { get; set; }

        public virtual ICollection<Comentario> Comentarios { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.Models
{
    // Tabla Comentarios (Database/04_Comentarios.sql)
    [Table("Comentarios")]
    public class Comentario
    {
        public Comentario()
        {
            // EF no aplica los DEFAULT de la base de datos, por eso se asigna aqui
            Fecha = DateTime.Now;
        }

        public int Id { get; set; }

        public int IdTicket { get; set; }

        public int IdUsuario { get; set; }

        [Required, StringLength(1000)]
        public string Texto { get; set; }

        public DateTime Fecha { get; set; }

        [ForeignKey("IdTicket")]
        public virtual Ticket Ticket { get; set; }

        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }
    }
}

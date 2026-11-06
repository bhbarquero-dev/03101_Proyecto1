using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.Models
{
    // Tabla EstadosTicket (Database/01_EstadosTicket.sql)
    [Table("EstadosTicket")]
    public class EstadoTicket
    {
        // Valores fijos insertados por el script
        public const int Abierto = 1;
        public const int EnProceso = 2;
        public const int Resuelto = 3;
        public const int Cerrado = 4;

        // Clase de Bootstrap con la que las vistas muestran cada estado
        public static string ClaseBadge(int idEstado)
        {
            switch (idEstado)
            {
                case Abierto: return "bg-primary";
                case EnProceso: return "bg-warning text-dark";
                case Resuelto: return "bg-success";
                default: return "bg-secondary";
            }
        }

        public EstadoTicket()
        {
            Tickets = new HashSet<Ticket>();
        }

        // Sin IDENTITY: el Id lo define el script, no la base de datos
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required, StringLength(30)]
        public string Nombre { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}

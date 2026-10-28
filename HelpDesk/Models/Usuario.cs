using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.Models
{
    // Tabla Usuarios (Database/02_Usuarios.sql)
    [Table("Usuarios")]
    public class Usuario
    {
        public const string RolUsuario = "Usuario";
        public const string RolAdministrador = "Administrador";

        public Usuario()
        {
            // EF no aplica los DEFAULT de la base de datos, por eso se asignan aqui
            Rol = RolUsuario;
            FechaRegistro = DateTime.Now;
            Estado = true;
            TicketsSolicitados = new HashSet<Ticket>();
            TicketsAsignados = new HashSet<Ticket>();
            Comentarios = new HashSet<Comentario>();
        }

        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }

        [Required, StringLength(150)]
        public string Email { get; set; }

        // Formato: PBKDF2-SHA256$iteraciones$sal$hash (ver Seguridad/PasswordHasher.cs)
        [Required, StringLength(255)]
        public string ContrasenaHash { get; set; }

        [Required, StringLength(20)]
        public string Rol { get; set; }

        public DateTime FechaRegistro { get; set; }

        // true = activo, false = baja logica
        public bool Estado { get; set; }

        [InverseProperty("Solicitante")]
        public virtual ICollection<Ticket> TicketsSolicitados { get; set; }

        [InverseProperty("Asignado")]
        public virtual ICollection<Ticket> TicketsAsignados { get; set; }

        public virtual ICollection<Comentario> Comentarios { get; set; }

        [NotMapped]
        public bool EsAdministrador
        {
            get { return Rol == RolAdministrador; }
        }
    }
}

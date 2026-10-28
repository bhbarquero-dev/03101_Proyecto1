using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using HelpDesk.Models;

namespace HelpDesk.Datos
{
    // Contexto de EF6 "Code First desde base de datos existente".
    // El esquema lo definen los scripts de la carpeta Database; EF solo lo lee y lo escribe.
    public class HelpDeskContext : DbContext
    {
        static HelpDeskContext()
        {
            // Sin inicializador: EF nunca crea, migra ni valida el esquema
            Database.SetInitializer<HelpDeskContext>(null);
        }

        // Cadena de conexion "HelpDesk" de Web.config
        public HelpDeskContext() : base("name=HelpDesk")
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<EstadoTicket> EstadosTicket { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Las columnas de fecha son DATETIME2(0) en los scripts
            modelBuilder.Properties<DateTime>().Configure(p => p.HasColumnType("datetime2"));

            // Igual que en la base de datos: solo Comentarios -> Tickets borra en cascada
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Entity<Comentario>()
                .HasRequired(c => c.Ticket)
                .WithMany(t => t.Comentarios)
                .HasForeignKey(c => c.IdTicket)
                .WillCascadeOnDelete(true);
        }
    }
}

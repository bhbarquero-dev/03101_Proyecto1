using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using HelpDesk.Models;

namespace HelpDesk.Seguridad
{
    // Datos del usuario autenticado guardados en la cookie de Forms Authentication.
    // El ticket va cifrado y firmado por ASP.NET, asi que no se puede alterar desde el navegador.
    public class SesionUsuario
    {
        public int Id { get; private set; }
        public string Nombre { get; private set; }
        public string Rol { get; private set; }

        public bool EsAdministrador
        {
            get { return Rol == Usuario.RolAdministrador; }
        }

        public static HttpCookie CrearCookie(Usuario usuario)
        {
            // UserData: Id|Rol|Nombre (el nombre va al final porque es el unico que podria contener '|')
            string datos = usuario.Id + "|" + usuario.Rol + "|" + usuario.Nombre;
            var ticket = new FormsAuthenticationTicket(1, usuario.Email, DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout), false, datos, FormsAuthentication.FormsCookiePath);

            return new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket))
            {
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL,
                Path = FormsAuthentication.FormsCookiePath
            };
        }

        // Devuelve null si no hay sesion iniciada
        public static SesionUsuario Obtener(IPrincipal usuario)
        {
            var identidad = usuario == null ? null : usuario.Identity as FormsIdentity;
            if (identidad == null || !identidad.IsAuthenticated) return null;

            string[] partes = identidad.Ticket.UserData.Split(new[] { '|' }, 3);
            int id;
            if (partes.Length != 3 || !int.TryParse(partes[0], out id)) return null;

            return new SesionUsuario { Id = id, Rol = partes[1], Nombre = partes[2] };
        }
    }
}

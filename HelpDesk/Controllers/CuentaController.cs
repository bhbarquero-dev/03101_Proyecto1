using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using HelpDesk.Datos;
using HelpDesk.Models;
using HelpDesk.Seguridad;

namespace HelpDesk.Controllers
{
    public class CuentaController : Controller
    {
        // Hash con formato valido que no corresponde a ninguna contrasena. Se verifica contra el
        // cuando el correo no existe, para que la respuesta tarde lo mismo y no revele que correos estan registrados.
        private const string HashFicticio = "PBKDF2-SHA256$600000$AAAAAAAAAAAAAAAAAAAAAA==$AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";

        // GET: /Cuenta/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Request.IsAuthenticated) return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        // POST: /Cuenta/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel modelo, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid) return View(modelo);

            string email = modelo.Email.Trim();
            Usuario usuario;
            using (var db = new HelpDeskContext())
            {
                usuario = db.Usuarios.SingleOrDefault(u => u.Email == email);
            }

            bool contrasenaValida = PasswordHasher.Verificar(modelo.Contrasena, usuario != null ? usuario.ContrasenaHash : HashFicticio);
            if (usuario == null || !contrasenaValida)
            {
                ModelState.AddModelError("", "Correo o contraseña incorrectos.");
                return View(modelo);
            }
            if (!usuario.Estado)
            {
                ModelState.AddModelError("", "La cuenta está inactiva. Contacte al área de soporte.");
                return View(modelo);
            }

            Response.Cookies.Add(SesionUsuario.CrearCookie(usuario));

            // Solo se redirige a direcciones de esta misma aplicacion
            if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        // POST: /Cuenta/Logout (por POST para que un enlace externo no pueda cerrar la sesion)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}

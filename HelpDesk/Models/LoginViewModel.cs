using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Models
{
    // Datos del formulario de inicio de sesion
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Ingrese su correo.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
        [StringLength(150)]
        [Display(Name = "Correo")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Ingrese su contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }
    }
}

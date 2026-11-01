using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HelpDesk.Models
{
    // Datos del formulario de nuevo ticket
    public class CrearTicketViewModel
    {
        // Lista fija: la base de datos guarda la categoria como texto, sin catalogo
        public static readonly string[] Categorias = { "Accesos", "Correo", "Hardware", "Red", "Software", "Otro" };

        // AllowHtml: un ticket puede citar codigo o mensajes de error con < y >; Razor los codifica al mostrarlos
        [AllowHtml]
        [Required(ErrorMessage = "Ingrese el título.")]
        [StringLength(150, ErrorMessage = "El título admite un máximo de 150 caracteres.")]
        [Display(Name = "Título")]
        public string Titulo { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Ingrese la descripción.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Seleccione la categoría.")]
        [Display(Name = "Categoría")]
        public string Categoria { get; set; }
    }
}

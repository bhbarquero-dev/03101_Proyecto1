using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HelpDesk.Models
{
    // Datos del formulario de nuevo comentario
    public class ComentarioViewModel
    {
        // AllowHtml: un comentario puede citar codigo o mensajes de error con < y >; se muestra como texto
        [AllowHtml]
        [Required(ErrorMessage = "Escriba el comentario.")]
        [StringLength(1000, ErrorMessage = "El comentario admite un máximo de 1000 caracteres.")]
        public string Texto { get; set; }
    }
}

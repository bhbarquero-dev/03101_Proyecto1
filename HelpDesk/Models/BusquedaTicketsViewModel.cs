using System.Web.Mvc;

namespace HelpDesk.Models
{
    // Criterios de la busqueda instantanea de tickets (HU-07)
    public class BusquedaTicketsViewModel
    {
        // AllowHtml: los titulos pueden contener < y >, por lo que la busqueda tambien;
        // el texto solo se usa como parametro de la consulta y la vista lo inserta con .text()
        [AllowHtml]
        public string Texto { get; set; }

        // 0 = todos los estados
        public int IdEstado { get; set; }
    }
}

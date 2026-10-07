using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Web.Models
{
    public class PrestamoReporte
    {
        public int PrestamoId { get; set; }

        [Display(Name = "Socio")]
        public string SocioNombre { get; set; } = string.Empty;

        [Display(Name = "DNI Socio")]
        public string SocioDNI { get; set; } = string.Empty;

        [Display(Name = "Libro Solicitado")]
        public string LibroTitulo { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Fecha Préstamo")]
        public DateTime FechaPrestamo { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha Límite")]
        public DateTime FechaLimite { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Web.Models
{
    public class Libro
    {
        public int LibroId { get; set; }

        [Required(ErrorMessage = "El título del libro es obligatorio.")]
        [StringLength(150, ErrorMessage = "El título no puede exceder los 150 caracteres.")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar un autor.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un autor válido de la lista.")]
        [Display(Name = "Autor")]
        public int AutorId { get; set; }

        [Display(Name = "Nombre de Autor")]
        public string? AutorNombre { get; set; }

        [Required(ErrorMessage = "El número de ejemplares es obligatorio.")]
        [Range(1, 1000, ErrorMessage = "El número de ejemplares debe estar entre 1 y 1000.")]
        [Display(Name = "Ejemplares")]
        public int Ejemplares { get; set; } = 1;

        [Range(0, 10000, ErrorMessage = "El precio debe estar entre 0 y 10000.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Precio (S/)")]
        public decimal? Precio { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Publicación")]
        public DateTime? FechaPublicacion { get; set; }

        public bool Activo { get; set; } = true;
    }
}

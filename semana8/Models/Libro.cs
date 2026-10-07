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

        [Display(Name = "ISBN")]
        [StringLength(20, ErrorMessage = "El ISBN no puede exceder los 20 caracteres.")]
        public string? ISBN { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un autor.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un autor válido de la lista.")]
        [Display(Name = "Autor")]
        public int AutorId { get; set; }

        [Display(Name = "Nombre de Autor")]
        public string? AutorNombre { get; set; }

        [Required(ErrorMessage = "El número de ejemplares es obligatorio.")]
        [Range(0, 1000, ErrorMessage = "El número de ejemplares debe estar entre 0 y 1000.")]
        [Display(Name = "Ejemplares")]
        public int Ejemplares { get; set; } = 1;

        public bool Activo { get; set; } = true;
    }
}

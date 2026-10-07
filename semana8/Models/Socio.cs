using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Web.Models
{
    public class Socio
    {
        public int SocioId { get; set; }

        [Required(ErrorMessage = "El nombre completo del socio es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "El DNI debe tener entre 8 y 15 caracteres.")]
        [Display(Name = "DNI")]
        public string DNI { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "El formato de correo electrónico no es válido.")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")]
        [Display(Name = "Correo Electrónico")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "El formato de teléfono no es válido.")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres.")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        public bool Activo { get; set; } = true;
    }
}

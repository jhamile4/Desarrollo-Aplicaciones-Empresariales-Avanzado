using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Models;

[Index(nameof(Dni), IsUnique = true)]
[Index(nameof(Correo), IsUnique = true)]
public class Asistente
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100, ErrorMessage = "Los nombres admiten como máximo 100 caracteres.")]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [StringLength(8)]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener exactamente 8 dígitos.")]
    [Display(Name = "DNI")]
    public string Dni { get; set; } = string.Empty;

    // Un índice único en SQL Server necesita longitud máxima (no puede ser nvarchar(max))
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(100, ErrorMessage = "El correo admite como máximo 100 caracteres.")]
    public string Correo { get; set; } = string.Empty;

    // Migración 3 (AgregarTelefonoAsistente): opcional
    [StringLength(9, ErrorMessage = "El teléfono admite como máximo 9 caracteres.")]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
}

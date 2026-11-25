using System.ComponentModel.DataAnnotations;

namespace Veterinaria.Entidades.Dtos;

// DTO de lectura: devuelve el nombre del propietario en lugar del objeto.
public class MascotaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public string? Raza { get; set; }
    public DateOnly FechaNacimiento { get; set; }
    public int EdadAnios { get; set; }
    public decimal Peso { get; set; }
    public int PropietarioId { get; set; }
    public string Propietario { get; set; } = string.Empty;
}

// DTO de entrada para POST.
public class MascotaCrearDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La especie es obligatoria.")]
    [StringLength(30, ErrorMessage = "La especie admite como máximo 30 caracteres.")]
    public string Especie { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "La raza admite como máximo 50 caracteres.")]
    public string? Raza { get; set; }

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    public DateOnly FechaNacimiento { get; set; }

    [Range(0.01, 200, ErrorMessage = "El peso debe estar entre 0.01 y 200 kg.")]
    public decimal Peso { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Indica un propietario válido.")]
    public int PropietarioId { get; set; }
}

// DTO de entrada para PUT (permite cambiar de propietario).
public class MascotaActualizarDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La especie es obligatoria.")]
    [StringLength(30, ErrorMessage = "La especie admite como máximo 30 caracteres.")]
    public string Especie { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "La raza admite como máximo 50 caracteres.")]
    public string? Raza { get; set; }

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    public DateOnly FechaNacimiento { get; set; }

    [Range(0.01, 200, ErrorMessage = "El peso debe estar entre 0.01 y 200 kg.")]
    public decimal Peso { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Indica un propietario válido.")]
    public int PropietarioId { get; set; }
}

using System;
using System.Collections.Generic;

namespace Veterinaria.Entidades;

public partial class Mascota
{
    public int IdMascota { get; set; }

    public string Nombre { get; set; } = null!;

    public string Especie { get; set; } = null!;

    public string? Raza { get; set; }

    public DateOnly FechaNacimiento { get; set; }

    public decimal Peso { get; set; }

    public int IdPropietario { get; set; }

    public virtual Propietario IdPropietarioNavigation { get; set; } = null!;
}

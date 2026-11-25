using System;
using System.Collections.Generic;

namespace Veterinaria.Entidades;

// Agregada a mano (Semana 14) siguiendo el mismo estilo que las entidades del scaffold.
public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Correo { get; set; } = null!;

    public string ClaveHash { get; set; } = null!;

    public string Rol { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }
}

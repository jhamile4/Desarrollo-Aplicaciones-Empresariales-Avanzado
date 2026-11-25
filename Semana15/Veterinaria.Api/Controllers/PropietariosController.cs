using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veterinaria.Entidades.Dtos;
using Veterinaria.Negocio.Excepciones;
using Veterinaria.Negocio.Servicios;

namespace Veterinaria.Api.Controllers;

[ApiController]
[Authorize]   // por defecto exige token; cada acción afina según la matriz de permisos
[Route("api/[controller]")]
[Produces("application/json")]
public class PropietariosController : ControllerBase
{
    private readonly IPropietarioServicio _servicio;

    public PropietariosController(IPropietarioServicio servicio)
    {
        _servicio = servicio;
    }

    // GET api/propietarios?buscar=torres&dni=45879632
    [HttpGet]   // solo autenticados (datos personales de los dueños)
    [ProducesResponseType<List<PropietarioDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<PropietarioDto>>> Listar([FromQuery] string? buscar, [FromQuery] string? dni)
    {
        return Ok(await _servicio.ListarAsync(buscar, dni));
    }

    // GET api/propietarios/5
    [HttpGet("{id:int}")]
    [ProducesResponseType<PropietarioDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PropietarioDto>> Obtener(int id)
    {
        var propietario = await _servicio.ObtenerAsync(id);
        if (propietario == null)
            return NotFound(new { mensaje = $"No existe el propietario {id}." });

        return Ok(propietario);
    }

    // GET api/propietarios/5/mascotas  (ruta anidada)
    [HttpGet("{id:int}/mascotas")]
    [ProducesResponseType<List<MascotaDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<MascotaDto>>> ListarMascotas(int id)
    {
        var mascotas = await _servicio.ListarMascotasAsync(id);
        if (mascotas == null)
            return NotFound(new { mensaje = $"No existe el propietario {id}." });

        return Ok(mascotas);
    }

    // POST api/propietarios
    [HttpPost]
    [Authorize(Roles = Roles.Recepcionista)]   // registrar dueños: solo recepción
    [ProducesResponseType<PropietarioDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PropietarioDto>> Crear(PropietarioCrearDto dto)
    {
        try
        {
            var creado = await _servicio.CrearAsync(dto);
            return CreatedAtAction(nameof(Obtener), new { id = creado.Id }, creado);
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // PUT api/propietarios/5
    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Recepcionista)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(int id, PropietarioActualizarDto dto)
    {
        if (!await _servicio.ActualizarAsync(id, dto))
            return NotFound(new { mensaje = $"No existe el propietario {id}." });

        return NoContent();
    }

    // DELETE api/propietarios/5
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Recepcionista)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            if (!await _servicio.EliminarAsync(id))
                return NotFound(new { mensaje = $"No existe el propietario {id}." });

            return NoContent();
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }
}

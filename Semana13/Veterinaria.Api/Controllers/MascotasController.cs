using Microsoft.AspNetCore.Mvc;
using Veterinaria.Entidades.Dtos;
using Veterinaria.Negocio.Excepciones;
using Veterinaria.Negocio.Servicios;

namespace Veterinaria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MascotasController : ControllerBase
{
    private readonly IMascotaServicio _servicio;

    public MascotasController(IMascotaServicio servicio)
    {
        _servicio = servicio;
    }

    // GET api/mascotas?especie=Perro&propietarioId=3&pesoMax=20
    [HttpGet]
    [ProducesResponseType<List<MascotaDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MascotaDto>>> Listar(
        [FromQuery] string? especie, [FromQuery] int? propietarioId, [FromQuery] decimal? pesoMax)
    {
        return Ok(await _servicio.ListarAsync(especie, propietarioId, pesoMax));
    }

    // GET api/mascotas/5
    [HttpGet("{id:int}")]
    [ProducesResponseType<MascotaDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MascotaDto>> Obtener(int id)
    {
        var mascota = await _servicio.ObtenerAsync(id);
        if (mascota == null)
            return NotFound(new { mensaje = $"No existe la mascota {id}." });

        return Ok(mascota);
    }

    // POST api/mascotas
    [HttpPost]
    [ProducesResponseType<MascotaDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MascotaDto>> Crear(MascotaCrearDto dto)
    {
        try
        {
            var creada = await _servicio.CrearAsync(dto);
            return CreatedAtAction(nameof(Obtener), new { id = creada.Id }, creada);
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // PUT api/mascotas/5
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Actualizar(int id, MascotaActualizarDto dto)
    {
        try
        {
            if (!await _servicio.ActualizarAsync(id, dto))
                return NotFound(new { mensaje = $"No existe la mascota {id}." });

            return NoContent();
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // DELETE api/mascotas/5
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id)
    {
        if (!await _servicio.EliminarAsync(id))
            return NotFound(new { mensaje = $"No existe la mascota {id}." });

        return NoContent();
    }
}

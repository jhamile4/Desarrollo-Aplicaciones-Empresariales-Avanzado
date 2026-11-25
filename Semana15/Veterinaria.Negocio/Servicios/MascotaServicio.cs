using AutoMapper;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Entidades;
using Veterinaria.Entidades.Dtos;
using Veterinaria.Negocio.Excepciones;

namespace Veterinaria.Negocio.Servicios;

// Reglas de negocio de Mascota y conversión entidad <-> DTO con AutoMapper.
public class MascotaServicio : IMascotaServicio
{
    // Regla 3: cupo máximo de mascotas por propietario.
    public const int MaximoMascotasPorPropietario = 3;

    private readonly IMascotaRepositorio _mascotas;
    private readonly IPropietarioRepositorio _propietarios;
    private readonly IMapper _mapper;

    public MascotaServicio(IMascotaRepositorio mascotas, IPropietarioRepositorio propietarios, IMapper mapper)
    {
        _mascotas = mascotas;
        _propietarios = propietarios;
        _mapper = mapper;
    }

    public async Task<List<MascotaDto>> ListarAsync(string? especie, int? propietarioId, decimal? pesoMax)
    {
        var lista = await _mascotas.ListarAsync(especie, propietarioId, pesoMax);
        return _mapper.Map<List<MascotaDto>>(lista);
    }

    public async Task<MascotaDto?> ObtenerAsync(int id)
    {
        var mascota = await _mascotas.ObtenerPorIdAsync(id);
        return mascota == null ? null : _mapper.Map<MascotaDto>(mascota);
    }

    public async Task<MascotaDto> CrearAsync(MascotaCrearDto dto)
    {
        await ValidarAsync(dto.Nombre, dto.FechaNacimiento, dto.PropietarioId, mascotaId: null, cambiaDePropietario: true);

        var mascota = _mapper.Map<Mascota>(dto);
        await _mascotas.AgregarAsync(mascota);

        // Se vuelve a leer con su propietario para devolver el nombre en el DTO.
        var creada = await _mascotas.ObtenerPorIdAsync(mascota.IdMascota);
        return _mapper.Map<MascotaDto>(creada!);
    }

    public async Task<bool> ActualizarAsync(int id, MascotaActualizarDto dto)
    {
        var mascota = await _mascotas.ObtenerParaEditarAsync(id);
        if (mascota == null)
            return false;

        await ValidarAsync(dto.Nombre, dto.FechaNacimiento, dto.PropietarioId, mascotaId: id,
            cambiaDePropietario: dto.PropietarioId != mascota.IdPropietario);

        // Actualiza el objeto existente con los datos del DTO.
        _mapper.Map(dto, mascota);
        await _mascotas.GuardarCambiosAsync();
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var mascota = await _mascotas.ObtenerParaEditarAsync(id);
        if (mascota == null)
            return false;

        await _mascotas.EliminarAsync(mascota);
        return true;
    }

    private async Task ValidarAsync(string nombre, DateOnly nacimiento, int propietarioId, int? mascotaId, bool cambiaDePropietario)
    {
        if (!await _propietarios.ExisteAsync(propietarioId))
            throw new ReglaNegocioException($"El propietario {propietarioId} no existe.");

        // Regla 4: la fecha de nacimiento no puede ser futura.
        if (nacimiento > DateOnly.FromDateTime(DateTime.Today))
            throw new ReglaNegocioException("La fecha de nacimiento no puede ser posterior a hoy.");

        // Regla 3: un propietario puede tener como máximo 3 mascotas.
        if (cambiaDePropietario
            && await _mascotas.ContarPorPropietarioAsync(propietarioId) >= MaximoMascotasPorPropietario)
        {
            throw new ReglaNegocioException(
                $"El propietario ya tiene {MaximoMascotasPorPropietario} mascotas registradas (máximo permitido).");
        }

        // Regla 5: un mismo propietario no puede tener dos mascotas con el mismo nombre.
        if (await _mascotas.ExisteNombreEnPropietarioAsync(nombre, propietarioId, mascotaId))
            throw new ReglaNegocioException($"El propietario ya tiene una mascota llamada {nombre}.");
    }
}

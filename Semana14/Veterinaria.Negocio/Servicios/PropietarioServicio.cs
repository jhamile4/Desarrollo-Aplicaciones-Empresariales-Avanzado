using AutoMapper;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Entidades;
using Veterinaria.Entidades.Dtos;
using Veterinaria.Negocio.Excepciones;

namespace Veterinaria.Negocio.Servicios;

// Reglas de negocio de Propietario y conversión entidad <-> DTO con AutoMapper.
public class PropietarioServicio : IPropietarioServicio
{
    private readonly IPropietarioRepositorio _propietarios;
    private readonly IMascotaRepositorio _mascotas;
    private readonly IMapper _mapper;

    public PropietarioServicio(IPropietarioRepositorio propietarios, IMascotaRepositorio mascotas, IMapper mapper)
    {
        _propietarios = propietarios;
        _mascotas = mascotas;
        _mapper = mapper;
    }

    public async Task<List<PropietarioDto>> ListarAsync(string? buscar, string? dni)
    {
        var lista = await _propietarios.ListarAsync(buscar, dni);
        return _mapper.Map<List<PropietarioDto>>(lista);
    }

    public async Task<PropietarioDto?> ObtenerAsync(int id)
    {
        var propietario = await _propietarios.ObtenerPorIdAsync(id);
        return propietario == null ? null : _mapper.Map<PropietarioDto>(propietario);
    }

    // Ruta anidada: null si el propietario no existe (404).
    public async Task<List<MascotaDto>?> ListarMascotasAsync(int id)
    {
        if (!await _propietarios.ExisteAsync(id))
            return null;

        var mascotas = await _mascotas.ListarPorPropietarioAsync(id);
        return _mapper.Map<List<MascotaDto>>(mascotas);
    }

    public async Task<PropietarioDto> CrearAsync(PropietarioCrearDto dto)
    {
        // Regla 1: el DNI no puede repetirse.
        if (await _propietarios.ExisteDniAsync(dto.Dni))
            throw new ReglaNegocioException($"Ya existe un propietario con el DNI {dto.Dni}.");

        var propietario = _mapper.Map<Propietario>(dto);
        await _propietarios.AgregarAsync(propietario);
        return _mapper.Map<PropietarioDto>(propietario);
    }

    public async Task<bool> ActualizarAsync(int id, PropietarioActualizarDto dto)
    {
        var propietario = await _propietarios.ObtenerParaEditarAsync(id);
        if (propietario == null)
            return false;

        // Actualiza el objeto existente (lo sigue EF) con los datos del DTO.
        _mapper.Map(dto, propietario);
        await _propietarios.GuardarCambiosAsync();
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var propietario = await _propietarios.ObtenerParaEditarAsync(id);
        if (propietario == null)
            return false;

        // Regla 2: no se elimina un propietario que todavía tiene mascotas.
        if (await _propietarios.TieneMascotasAsync(id))
            throw new ReglaNegocioException(
                $"No se puede eliminar al propietario: tiene {propietario.Mascota.Count} mascota(s) registrada(s).");

        await _propietarios.EliminarAsync(propietario);
        return true;
    }
}

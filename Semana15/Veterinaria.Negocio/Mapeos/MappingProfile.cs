using AutoMapper;
using Veterinaria.Entidades;
using Veterinaria.Entidades.Dtos;

namespace Veterinaria.Negocio.Mapeos;

// Único perfil de AutoMapper con todos los mapeos entre entidades y DTOs.
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ---------- Propietario ----------

        // 1) Entidad -> DTO de lectura (lo que la API devuelve)
        CreateMap<Propietario, PropietarioDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.IdPropietario))
            .ForMember(d => d.NombreCompleto, o => o.MapFrom(s => s.Nombres + " " + s.Apellidos))
            .ForMember(d => d.CantidadMascotas, o => o.MapFrom(s => s.Mascota.Count));

        // 2) DTO de entrada (POST) -> Entidad nueva
        CreateMap<PropietarioCrearDto, Propietario>()
            .ForMember(d => d.IdPropietario, o => o.Ignore())
            .ForMember(d => d.FechaRegistro, o => o.Ignore())
            .ForMember(d => d.Mascota, o => o.Ignore());

        // 3) DTO de entrada (PUT) -> Entidad existente (se usa con Map(origen, destino))
        CreateMap<PropietarioActualizarDto, Propietario>()
            .ForMember(d => d.IdPropietario, o => o.Ignore())
            .ForMember(d => d.Dni, o => o.Ignore())
            .ForMember(d => d.FechaRegistro, o => o.Ignore())
            .ForMember(d => d.Mascota, o => o.Ignore());

        // ---------- Mascota ----------

        // 4) Entidad -> DTO de lectura: nombre del propietario en lugar del objeto
        CreateMap<Mascota, MascotaDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.IdMascota))
            .ForMember(d => d.PropietarioId, o => o.MapFrom(s => s.IdPropietario))
            .ForMember(d => d.Propietario, o => o.MapFrom(s =>
                s.IdPropietarioNavigation.Nombres + " " + s.IdPropietarioNavigation.Apellidos))
            .ForMember(d => d.EdadAnios, o => o.MapFrom(s => CalcularEdad(s.FechaNacimiento)));

        // 5) DTO de entrada (POST) -> Entidad nueva
        CreateMap<MascotaCrearDto, Mascota>()
            .ForMember(d => d.IdMascota, o => o.Ignore())
            .ForMember(d => d.IdPropietario, o => o.MapFrom(s => s.PropietarioId))
            .ForMember(d => d.IdPropietarioNavigation, o => o.Ignore());

        // 6) DTO de entrada (PUT) -> Entidad existente
        CreateMap<MascotaActualizarDto, Mascota>()
            .ForMember(d => d.IdMascota, o => o.Ignore())
            .ForMember(d => d.IdPropietario, o => o.MapFrom(s => s.PropietarioId))
            .ForMember(d => d.IdPropietarioNavigation, o => o.Ignore());

        // ---------- Usuario (Semana 14) ----------

        // 7) Entidad -> DTO de salida del registro: nunca expone ClaveHash
        CreateMap<Usuario, UsuarioDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.IdUsuario));

        // 8) DTO de registro -> Entidad nueva: el hash y el rol los pone el servicio, no el cliente
        CreateMap<RegistroDto, Usuario>()
            .ForMember(d => d.IdUsuario, o => o.Ignore())
            .ForMember(d => d.ClaveHash, o => o.Ignore())
            .ForMember(d => d.Rol, o => o.Ignore())
            .ForMember(d => d.FechaRegistro, o => o.Ignore());
    }

    private static int CalcularEdad(DateOnly nacimiento)
    {
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        var edad = hoy.Year - nacimiento.Year;
        if (nacimiento > hoy.AddYears(-edad))
            edad--;
        return Math.Max(edad, 0);
    }
}

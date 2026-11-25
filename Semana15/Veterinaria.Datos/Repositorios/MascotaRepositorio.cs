using Microsoft.EntityFrameworkCore;
using Veterinaria.Datos.Models;
using Veterinaria.Entidades;

namespace Veterinaria.Datos.Repositorios;

// Acceso a datos de Mascota: solo LINQ y SaveChangesAsync, sin reglas de negocio.
public class MascotaRepositorio : IMascotaRepositorio
{
    private readonly VeterinariaContext _context;

    public MascotaRepositorio(VeterinariaContext context)
    {
        _context = context;
    }

    // Filtros opcionales por query string: especie, propietario y peso máximo.
    public async Task<List<Mascota>> ListarAsync(string? especie, int? propietarioId, decimal? pesoMax)
    {
        var consulta = _context.Mascota
            .AsNoTracking()
            .Include(m => m.IdPropietarioNavigation)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(especie))
            consulta = consulta.Where(m => m.Especie == especie);

        if (propietarioId.HasValue)
            consulta = consulta.Where(m => m.IdPropietario == propietarioId.Value);

        if (pesoMax.HasValue)
            consulta = consulta.Where(m => m.Peso <= pesoMax.Value);

        return await consulta.OrderBy(m => m.Nombre).ToListAsync();
    }

    public async Task<List<Mascota>> ListarPorPropietarioAsync(int propietarioId)
    {
        return await _context.Mascota
            .AsNoTracking()
            .Include(m => m.IdPropietarioNavigation)
            .Where(m => m.IdPropietario == propietarioId)
            .OrderBy(m => m.Nombre)
            .ToListAsync();
    }

    public async Task<Mascota?> ObtenerPorIdAsync(int id)
    {
        return await _context.Mascota
            .AsNoTracking()
            .Include(m => m.IdPropietarioNavigation)
            .FirstOrDefaultAsync(m => m.IdMascota == id);
    }

    public async Task<Mascota?> ObtenerParaEditarAsync(int id)
    {
        return await _context.Mascota.FirstOrDefaultAsync(m => m.IdMascota == id);
    }

    public async Task<int> ContarPorPropietarioAsync(int propietarioId)
    {
        return await _context.Mascota.CountAsync(m => m.IdPropietario == propietarioId);
    }

    public async Task<bool> ExisteNombreEnPropietarioAsync(string nombre, int propietarioId, int? excluirId = null)
    {
        return await _context.Mascota.AnyAsync(m => m.IdPropietario == propietarioId
                                                    && m.Nombre == nombre
                                                    && (excluirId == null || m.IdMascota != excluirId));
    }

    public async Task AgregarAsync(Mascota mascota)
    {
        _context.Mascota.Add(mascota);
        await _context.SaveChangesAsync();
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Mascota mascota)
    {
        _context.Mascota.Remove(mascota);
        await _context.SaveChangesAsync();
    }
}

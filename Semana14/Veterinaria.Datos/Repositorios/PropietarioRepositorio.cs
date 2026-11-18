using Microsoft.EntityFrameworkCore;
using Veterinaria.Datos.Models;
using Veterinaria.Entidades;

namespace Veterinaria.Datos.Repositorios;

// Acceso a datos de Propietario: solo LINQ y SaveChangesAsync, sin reglas de negocio.
public class PropietarioRepositorio : IPropietarioRepositorio
{
    private readonly VeterinariaContext _context;

    public PropietarioRepositorio(VeterinariaContext context)
    {
        _context = context;
    }

    // Filtros opcionales: texto en nombres/apellidos y DNI exacto.
    public async Task<List<Propietario>> ListarAsync(string? buscar, string? dni)
    {
        var consulta = _context.Propietario
            .AsNoTracking()
            .Include(p => p.Mascota)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
            consulta = consulta.Where(p => p.Nombres.Contains(buscar) || p.Apellidos.Contains(buscar));

        if (!string.IsNullOrWhiteSpace(dni))
            consulta = consulta.Where(p => p.Dni == dni);

        return await consulta
            .OrderBy(p => p.Apellidos)
            .ThenBy(p => p.Nombres)
            .ToListAsync();
    }

    public async Task<Propietario?> ObtenerPorIdAsync(int id)
    {
        return await _context.Propietario
            .AsNoTracking()
            .Include(p => p.Mascota)
            .FirstOrDefaultAsync(p => p.IdPropietario == id);
    }

    // Con seguimiento (tracking) para que SaveChangesAsync detecte los cambios.
    public async Task<Propietario?> ObtenerParaEditarAsync(int id)
    {
        return await _context.Propietario
            .Include(p => p.Mascota)
            .FirstOrDefaultAsync(p => p.IdPropietario == id);
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _context.Propietario.AnyAsync(p => p.IdPropietario == id);
    }

    public async Task<bool> ExisteDniAsync(string dni)
    {
        return await _context.Propietario.AnyAsync(p => p.Dni == dni);
    }

    public async Task<bool> TieneMascotasAsync(int id)
    {
        return await _context.Mascota.AnyAsync(m => m.IdPropietario == id);
    }

    public async Task AgregarAsync(Propietario propietario)
    {
        _context.Propietario.Add(propietario);
        await _context.SaveChangesAsync();
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Propietario propietario)
    {
        _context.Propietario.Remove(propietario);
        await _context.SaveChangesAsync();
    }
}

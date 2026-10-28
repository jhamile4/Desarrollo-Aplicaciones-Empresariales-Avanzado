using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

// Toda la lógica de acceso a datos de las inscripciones (LINQ + SaveChangesAsync).
public class InscripcionRepositorio
{
    private readonly EventosContext _context;

    public InscripcionRepositorio(EventosContext context)
    {
        _context = context;
    }

    // Inscripciones con su evento y asistente, las más recientes primero.
    public async Task<List<Inscripcion>> ListarAsync(string? estado)
    {
        var consulta = _context.Inscripciones
            .AsNoTracking()
            .Include(i => i.Evento)
            .Include(i => i.Asistente)
            .AsQueryable();

        if (!string.IsNullOrEmpty(estado))
            consulta = consulta.Where(i => i.Estado == estado);

        return await consulta
            .OrderByDescending(i => i.FechaInscripcion)
            .ToListAsync();
    }

    // Datos de las listas desplegables: eventos y asistentes activos.
    public async Task<List<Evento>> ListarEventosActivosAsync()
    {
        return await _context.Eventos
            .AsNoTracking()
            .Where(e => e.Activo)
            .OrderBy(e => e.Fecha)
            .ToListAsync();
    }

    public async Task<List<Asistente>> ListarAsistentesActivosAsync()
    {
        return await _context.Asistentes
            .AsNoTracking()
            .Where(a => a.Activo)
            .OrderBy(a => a.Nombres)
            .ToListAsync();
    }

    public async Task<Evento?> ObtenerEventoAsync(int eventoId)
    {
        return await _context.Eventos
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == eventoId && e.Activo);
    }

    public async Task<bool> YaInscritoAsync(int eventoId, int asistenteId)
    {
        return await _context.Inscripciones
            .AsNoTracking()
            .AnyAsync(i => i.EventoId == eventoId
                           && i.AsistenteId == asistenteId
                           && i.Estado == "Confirmada");
    }

    public async Task<int> ContarConfirmadasAsync(int eventoId)
    {
        return await _context.Inscripciones
            .AsNoTracking()
            .CountAsync(i => i.EventoId == eventoId && i.Estado == "Confirmada");
    }

    public async Task<int> ContarPorEstadoAsync(string estado)
    {
        return await _context.Inscripciones.AsNoTracking().CountAsync(i => i.Estado == estado);
    }

    public async Task CrearAsync(Inscripcion inscripcion)
    {
        inscripcion.Estado = "Confirmada";
        inscripcion.FechaInscripcion = DateTime.Now;
        _context.Inscripciones.Add(inscripcion);
        await _context.SaveChangesAsync();
    }

    // Solo se cancelan las inscripciones Confirmada.
    public async Task<bool> CancelarAsync(int id)
    {
        var inscripcion = await _context.Inscripciones
            .FirstOrDefaultAsync(i => i.Id == id && i.Estado == "Confirmada");
        if (inscripcion == null)
            return false;

        inscripcion.Estado = "Cancelada";
        await _context.SaveChangesAsync();
        return true;
    }
}

using Hotel.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Web.Repositorios;

// Toda la lógica de acceso a datos de las habitaciones (LINQ + SaveChangesAsync).
public class HabitacionRepositorio
{
    private readonly HotelContext _context;

    public HabitacionRepositorio(HotelContext context)
    {
        _context = context;
    }

    // Habitaciones activas con su tipo, filtro opcional por tipo y orden por número.
    public async Task<List<Habitacion>> ListarActivasAsync(int? idTipo)
    {
        var consulta = _context.Habitacions
            .AsNoTracking()
            .Include(h => h.IdTipoHabitacionNavigation)
            .Where(h => h.Activo);

        if (idTipo.HasValue)
            consulta = consulta.Where(h => h.IdTipoHabitacion == idTipo.Value);

        return await consulta.OrderBy(h => h.Numero).ToListAsync();
    }

    public async Task<Habitacion?> ObtenerPorIdAsync(int id)
    {
        return await _context.Habitacions
            .AsNoTracking()
            .Include(h => h.IdTipoHabitacionNavigation)
            .FirstOrDefaultAsync(h => h.IdHabitacion == id && h.Activo);
    }

    // Próximas reservas (no canceladas) de una habitación, para la vista Details.
    public async Task<List<Reserva>> ListarProximasReservasAsync(int idHabitacion)
    {
        var hoy = DateOnly.FromDateTime(DateTime.Today);

        return await _context.Reservas
            .AsNoTracking()
            .Include(r => r.IdHuespedNavigation)
            .Where(r => r.IdHabitacion == idHabitacion
                        && r.Estado != "Cancelada"
                        && r.FechaSalida >= hoy)
            .OrderBy(r => r.FechaIngreso)
            .ToListAsync();
    }

    // El número es UNIQUE en la tabla (incluye habitaciones inactivas).
    public async Task<bool> ExisteNumeroAsync(string numero, int? excluirId = null)
    {
        return await _context.Habitacions
            .AsNoTracking()
            .AnyAsync(h => h.Numero == numero
                           && (excluirId == null || h.IdHabitacion != excluirId));
    }

    // Datos de la lista desplegable de tipos de habitación.
    public async Task<List<TipoHabitacion>> ListarTiposAsync()
    {
        return await _context.TipoHabitacions
            .AsNoTracking()
            .Where(t => t.Activo)
            .OrderBy(t => t.PrecioNoche)
            .ToListAsync();
    }

    public async Task<int> ContarActivasAsync()
    {
        return await _context.Habitacions.AsNoTracking().CountAsync(h => h.Activo);
    }

    public async Task CrearAsync(Habitacion habitacion)
    {
        habitacion.Activo = true;
        _context.Habitacions.Add(habitacion);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ActualizarAsync(Habitacion habitacion)
    {
        var actual = await _context.Habitacions
            .FirstOrDefaultAsync(h => h.IdHabitacion == habitacion.IdHabitacion && h.Activo);
        if (actual == null)
            return false;

        actual.Numero = habitacion.Numero;
        actual.Piso = habitacion.Piso;
        actual.IdTipoHabitacion = habitacion.IdTipoHabitacion;
        await _context.SaveChangesAsync();
        return true;
    }

    // Eliminación lógica: Activo = 0, nunca un DELETE físico.
    public async Task<bool> EliminarAsync(int id)
    {
        var habitacion = await _context.Habitacions
            .FirstOrDefaultAsync(h => h.IdHabitacion == id && h.Activo);
        if (habitacion == null)
            return false;

        habitacion.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }
}

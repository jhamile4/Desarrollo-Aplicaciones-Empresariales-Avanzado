using Hotel.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Web.Repositorios;

// Toda la lógica de acceso a datos de las reservas (LINQ + SaveChangesAsync).
public class ReservaRepositorio
{
    private readonly HotelContext _context;

    public ReservaRepositorio(HotelContext context)
    {
        _context = context;
    }

    // Habitaciones activas libres en el rango pedido, en UNA sola consulta LINQ.
    // Una reserva se cruza si no está Cancelada, entra antes de la salida pedida
    // y sale después del ingreso pedido.
    public async Task<List<Habitacion>> ListarDisponiblesAsync(DateOnly ingreso, DateOnly salida)
    {
        return await _context.Habitacions
            .AsNoTracking()
            .Include(h => h.IdTipoHabitacionNavigation)
            .Where(h => h.Activo
                        && !h.Reservas.Any(r => r.Estado != "Cancelada"
                                                && r.FechaIngreso < salida
                                                && r.FechaSalida > ingreso))
            .OrderBy(h => h.Numero)
            .ToListAsync();
    }

    // Misma regla de cruce, para validar una habitación concreta al reservar.
    public async Task<bool> EstaReservadaAsync(int idHabitacion, DateOnly ingreso, DateOnly salida)
    {
        return await _context.Reservas
            .AsNoTracking()
            .AnyAsync(r => r.IdHabitacion == idHabitacion
                           && r.Estado != "Cancelada"
                           && r.FechaIngreso < salida
                           && r.FechaSalida > ingreso);
    }

    // Reservas con huésped, habitación y tipo (para calcular el total).
    public async Task<List<Reserva>> ListarAsync(string? estado)
    {
        var consulta = _context.Reservas
            .AsNoTracking()
            .Include(r => r.IdHuespedNavigation)
            .Include(r => r.IdHabitacionNavigation)
                .ThenInclude(h => h.IdTipoHabitacionNavigation)
            .AsQueryable();

        if (!string.IsNullOrEmpty(estado))
            consulta = consulta.Where(r => r.Estado == estado);

        return await consulta
            .OrderByDescending(r => r.FechaIngreso)
            .ThenBy(r => r.IdReserva)
            .ToListAsync();
    }

    // Datos de las listas desplegables: huéspedes y habitaciones activas.
    public async Task<List<Huesped>> ListarHuespedesActivosAsync()
    {
        return await _context.Huespeds
            .AsNoTracking()
            .Where(h => h.Activo)
            .OrderBy(h => h.Apellidos)
            .ThenBy(h => h.Nombres)
            .ToListAsync();
    }

    public async Task<List<Habitacion>> ListarHabitacionesActivasAsync()
    {
        return await _context.Habitacions
            .AsNoTracking()
            .Include(h => h.IdTipoHabitacionNavigation)
            .Where(h => h.Activo)
            .OrderBy(h => h.Numero)
            .ToListAsync();
    }

    public async Task<int> ContarPorEstadoAsync(string estado)
    {
        return await _context.Reservas.AsNoTracking().CountAsync(r => r.Estado == estado);
    }

    public async Task CrearAsync(Reserva reserva)
    {
        reserva.Estado = "Pendiente";
        _context.Reservas.Add(reserva);
        await _context.SaveChangesAsync();
    }

    // Solo se cancelan las reservas Pendiente o Confirmada.
    public async Task<bool> CancelarAsync(int id)
    {
        var reserva = await _context.Reservas
            .FirstOrDefaultAsync(r => r.IdReserva == id
                                      && (r.Estado == "Pendiente" || r.Estado == "Confirmada"));
        if (reserva == null)
            return false;

        reserva.Estado = "Cancelada";
        await _context.SaveChangesAsync();
        return true;
    }
}

using Hotel.Web.Models;
using Hotel.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel.Web.Controllers;

public class ReservasController : Controller
{
    private readonly ReservaRepositorio _repositorio;

    public ReservasController(ReservaRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    // GET: /Reservas?estado=Pendiente
    public async Task<IActionResult> Index(string? estado)
    {
        ViewBag.Estado = estado;
        var reservas = await _repositorio.ListarAsync(estado);
        return View(reservas);
    }

    // GET: /Reservas/Disponibles?ingreso=2026-10-22&salida=2026-10-25
    public async Task<IActionResult> Disponibles(DateOnly? ingreso, DateOnly? salida)
    {
        ViewBag.Ingreso = ingreso;
        ViewBag.Salida = salida;

        if (ingreso == null || salida == null)
            return View(null);

        if (salida <= ingreso)
        {
            ModelState.AddModelError(string.Empty, "La fecha de salida debe ser posterior a la de ingreso.");
            return View(null);
        }

        ViewBag.Noches = salida.Value.DayNumber - ingreso.Value.DayNumber;
        var habitaciones = await _repositorio.ListarDisponiblesAsync(ingreso.Value, salida.Value);
        return View(habitaciones);
    }

    // GET: /Reservas/Create  (opcionalmente con datos que vienen de Disponibles)
    public async Task<IActionResult> Create(int? idHabitacion, DateOnly? ingreso, DateOnly? salida)
    {
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        var reserva = new Reserva
        {
            IdHabitacion = idHabitacion ?? 0,
            FechaIngreso = ingreso ?? hoy,
            FechaSalida = salida ?? hoy.AddDays(1)
        };

        await CargarListasAsync(reserva);
        return View(reserva);
    }

    // POST: /Reservas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Reserva reserva)
    {
        // Navegaciones y Estado no vienen del formulario
        ModelState.Remove(nameof(Reserva.IdHuespedNavigation));
        ModelState.Remove(nameof(Reserva.IdHabitacionNavigation));
        ModelState.Remove(nameof(Reserva.Estado));

        var hoy = DateOnly.FromDateTime(DateTime.Today);

        if (reserva.IdHuesped <= 0)
            ModelState.AddModelError(nameof(Reserva.IdHuesped), "Selecciona un huésped.");

        if (reserva.IdHabitacion <= 0)
            ModelState.AddModelError(nameof(Reserva.IdHabitacion), "Selecciona una habitación.");

        if (reserva.FechaIngreso < hoy)
            ModelState.AddModelError(nameof(Reserva.FechaIngreso), "La fecha de ingreso no puede ser anterior a hoy.");

        if (reserva.FechaSalida <= reserva.FechaIngreso)
            ModelState.AddModelError(nameof(Reserva.FechaSalida), "La fecha de salida debe ser posterior a la de ingreso.");

        if (reserva.Observaciones?.Length > 200)
            ModelState.AddModelError(nameof(Reserva.Observaciones), "Las observaciones admiten como máximo 200 caracteres.");

        if (ModelState.IsValid
            && await _repositorio.EstaReservadaAsync(reserva.IdHabitacion, reserva.FechaIngreso, reserva.FechaSalida))
        {
            ModelState.AddModelError(nameof(Reserva.IdHabitacion),
                "La habitación ya está reservada en ese rango de fechas.");
        }

        if (!ModelState.IsValid)
        {
            await CargarListasAsync(reserva);
            return View(reserva);
        }

        await _repositorio.CrearAsync(reserva);
        TempData["Exito"] = $"Reserva registrada del {reserva.FechaIngreso:dd/MM/yyyy} al {reserva.FechaSalida:dd/MM/yyyy} con estado Pendiente.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Reservas/Cancelar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        if (await _repositorio.CancelarAsync(id))
            TempData["Exito"] = $"La reserva #{id} fue cancelada.";
        else
            TempData["Error"] = "Solo se pueden cancelar reservas Pendientes o Confirmadas.";

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync(Reserva reserva)
    {
        var huespedes = await _repositorio.ListarHuespedesActivosAsync();
        var habitaciones = await _repositorio.ListarHabitacionesActivasAsync();

        ViewBag.IdHuesped = new SelectList(
            huespedes.Select(h => new { h.IdHuesped, Texto = $"{h.Apellidos}, {h.Nombres} · DNI {h.Dni}" }),
            "IdHuesped", "Texto", reserva.IdHuesped);

        ViewBag.IdHabitacion = new SelectList(
            habitaciones.Select(h => new
            {
                h.IdHabitacion,
                Texto = $"Hab. {h.Numero} · {h.IdTipoHabitacionNavigation.Nombre} · S/ {h.IdTipoHabitacionNavigation.PrecioNoche:N2}"
            }),
            "IdHabitacion", "Texto", reserva.IdHabitacion);
    }
}

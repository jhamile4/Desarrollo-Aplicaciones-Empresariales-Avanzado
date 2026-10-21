using Hotel.Web.Models;
using Hotel.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel.Web.Controllers;

public class HabitacionesController : Controller
{
    private readonly HabitacionRepositorio _repositorio;

    public HabitacionesController(HabitacionRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    // GET: /Habitaciones?idTipo=2
    public async Task<IActionResult> Index(int? idTipo)
    {
        var tipos = await _repositorio.ListarTiposAsync();
        ViewBag.Tipos = new SelectList(tipos, "IdTipoHabitacion", "Nombre", idTipo);
        ViewBag.IdTipo = idTipo;

        var habitaciones = await _repositorio.ListarActivasAsync(idTipo);
        return View(habitaciones);
    }

    // GET: /Habitaciones/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var habitacion = await _repositorio.ObtenerPorIdAsync(id);
        if (habitacion == null)
            return NotFound();

        ViewBag.Reservas = await _repositorio.ListarProximasReservasAsync(id);
        return View(habitacion);
    }

    // GET: /Habitaciones/Create
    public async Task<IActionResult> Create()
    {
        await CargarTiposAsync();
        return View(new Habitacion { Piso = 1 });
    }

    // POST: /Habitaciones/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Habitacion habitacion)
    {
        QuitarNavegaciones();
        await ValidarAsync(habitacion, excluirId: null);

        if (!ModelState.IsValid)
        {
            await CargarTiposAsync(habitacion.IdTipoHabitacion);
            return View(habitacion);
        }

        await _repositorio.CrearAsync(habitacion);
        TempData["Exito"] = $"La habitación {habitacion.Numero} se registró correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Habitaciones/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var habitacion = await _repositorio.ObtenerPorIdAsync(id);
        if (habitacion == null)
            return NotFound();

        await CargarTiposAsync(habitacion.IdTipoHabitacion);
        return View(habitacion);
    }

    // POST: /Habitaciones/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Habitacion habitacion)
    {
        if (id != habitacion.IdHabitacion)
            return BadRequest();

        QuitarNavegaciones();
        await ValidarAsync(habitacion, excluirId: id);

        if (!ModelState.IsValid)
        {
            await CargarTiposAsync(habitacion.IdTipoHabitacion);
            return View(habitacion);
        }

        if (!await _repositorio.ActualizarAsync(habitacion))
            return NotFound();

        TempData["Exito"] = $"La habitación {habitacion.Numero} se actualizó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Habitaciones/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var habitacion = await _repositorio.ObtenerPorIdAsync(id);
        if (habitacion == null)
            return NotFound();

        return View(habitacion);
    }

    // POST: /Habitaciones/Delete/5  (eliminación lógica)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (await _repositorio.EliminarAsync(id))
            TempData["Exito"] = "La habitación se dio de baja (eliminación lógica).";
        else
            TempData["Error"] = "La habitación no existe o ya estaba dada de baja.";

        return RedirectToAction(nameof(Index));
    }

    // Las propiedades de navegación no llegan en el formulario: se quitan de la validación.
    private void QuitarNavegaciones()
    {
        ModelState.Remove(nameof(Habitacion.IdTipoHabitacionNavigation));
        ModelState.Remove(nameof(Habitacion.Reservas));
    }

    private async Task ValidarAsync(Habitacion habitacion, int? excluirId)
    {
        if (habitacion.Piso < 1)
            ModelState.AddModelError(nameof(Habitacion.Piso), "El piso debe ser 1 o mayor.");

        if (!string.IsNullOrWhiteSpace(habitacion.Numero)
            && await _repositorio.ExisteNumeroAsync(habitacion.Numero.Trim(), excluirId))
        {
            ModelState.AddModelError(nameof(Habitacion.Numero),
                $"Ya existe una habitación con el número {habitacion.Numero}.");
        }
    }

    private async Task CargarTiposAsync(int? seleccionado = null)
    {
        var tipos = await _repositorio.ListarTiposAsync();
        ViewBag.IdTipoHabitacion = new SelectList(
            tipos.Select(t => new { t.IdTipoHabitacion, Texto = $"{t.Nombre} · S/ {t.PrecioNoche:N2} por noche" }),
            "IdTipoHabitacion", "Texto", seleccionado);
    }
}

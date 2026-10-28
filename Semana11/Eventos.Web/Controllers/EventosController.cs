using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class EventosController : Controller
{
    private static readonly string[] Modalidades = { "Presencial", "Virtual" };

    private readonly EventoRepositorio _repositorio;

    public EventosController(EventoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    // GET: /Eventos?categoriaId=1
    public async Task<IActionResult> Index(int? categoriaId)
    {
        var categorias = await _repositorio.ListarCategoriasActivasAsync();
        ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre", categoriaId);
        ViewBag.CategoriaId = categoriaId;

        var eventos = await _repositorio.ListarActivosAsync(categoriaId);
        return View(eventos);
    }

    // GET: /Eventos/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var evento = await _repositorio.ObtenerPorIdAsync(id);
        if (evento == null)
            return NotFound();

        ViewBag.Inscritos = await _repositorio.ListarInscritosAsync(id);
        return View(evento);
    }

    // GET: /Eventos/Create
    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View(new Evento
        {
            Fecha = DateTime.Today.AddDays(7).AddHours(18),
            Aforo = 50
        });
    }

    // POST: /Eventos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Evento evento)
    {
        if (!ModelState.IsValid)
        {
            await CargarListasAsync(evento);
            return View(evento);
        }

        await _repositorio.CrearAsync(evento);
        TempData["Exito"] = $"El evento \"{evento.Titulo}\" se registró correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Eventos/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var evento = await _repositorio.ObtenerPorIdAsync(id);
        if (evento == null)
            return NotFound();

        await CargarListasAsync(evento);
        return View(evento);
    }

    // POST: /Eventos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Evento evento)
    {
        if (id != evento.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            await CargarListasAsync(evento);
            return View(evento);
        }

        if (!await _repositorio.ActualizarAsync(evento))
            return NotFound();

        TempData["Exito"] = $"El evento \"{evento.Titulo}\" se actualizó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Eventos/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var evento = await _repositorio.ObtenerPorIdAsync(id);
        if (evento == null)
            return NotFound();

        return View(evento);
    }

    // POST: /Eventos/Delete/5  (eliminación lógica)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (await _repositorio.EliminarAsync(id))
            TempData["Exito"] = "El evento se dio de baja (eliminación lógica).";
        else
            TempData["Error"] = "El evento no existe o ya estaba dado de baja.";

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync(Evento? evento = null)
    {
        var categorias = await _repositorio.ListarCategoriasActivasAsync();
        ViewBag.CategoriaId = new SelectList(categorias, "Id", "Nombre", evento?.CategoriaId);
        ViewBag.Modalidad = new SelectList(Modalidades, evento?.Modalidad ?? "Presencial");
    }
}

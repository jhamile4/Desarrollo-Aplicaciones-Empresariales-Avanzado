using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class InscripcionesController : Controller
{
    private readonly InscripcionRepositorio _repositorio;

    public InscripcionesController(InscripcionRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    // GET: /Inscripciones?estado=Confirmada
    public async Task<IActionResult> Index(string? estado)
    {
        ViewBag.Estado = estado;
        var inscripciones = await _repositorio.ListarAsync(estado);
        return View(inscripciones);
    }

    // GET: /Inscripciones/Create  (opcionalmente con el evento preseleccionado)
    public async Task<IActionResult> Create(int? eventoId)
    {
        var inscripcion = new Inscripcion { EventoId = eventoId ?? 0 };
        await CargarListasAsync(inscripcion);
        return View(inscripcion);
    }

    // POST: /Inscripciones/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Inscripcion inscripcion)
    {
        if (ModelState.IsValid)
        {
            var evento = await _repositorio.ObtenerEventoAsync(inscripcion.EventoId);

            if (evento == null)
            {
                ModelState.AddModelError(nameof(Inscripcion.EventoId), "El evento no existe o no está activo.");
            }
            else
            {
                if (evento.Fecha < DateTime.Now)
                    ModelState.AddModelError(nameof(Inscripcion.EventoId), "El evento ya pasó; no se admiten inscripciones.");

                if (await _repositorio.YaInscritoAsync(inscripcion.EventoId, inscripcion.AsistenteId))
                    ModelState.AddModelError(nameof(Inscripcion.AsistenteId), "El asistente ya está inscrito en este evento.");

                var confirmadas = await _repositorio.ContarConfirmadasAsync(inscripcion.EventoId);
                if (confirmadas >= evento.Aforo)
                    ModelState.AddModelError(nameof(Inscripcion.EventoId), $"El aforo está completo ({confirmadas}/{evento.Aforo}).");
            }
        }

        if (!ModelState.IsValid)
        {
            await CargarListasAsync(inscripcion);
            return View(inscripcion);
        }

        await _repositorio.CrearAsync(inscripcion);
        TempData["Exito"] = "La inscripción se registró con estado Confirmada.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Inscripciones/Cancelar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        if (await _repositorio.CancelarAsync(id))
            TempData["Exito"] = $"La inscripción #{id} fue cancelada.";
        else
            TempData["Error"] = "Solo se pueden cancelar inscripciones Confirmadas.";

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync(Inscripcion inscripcion)
    {
        var eventos = await _repositorio.ListarEventosActivosAsync();
        var asistentes = await _repositorio.ListarAsistentesActivosAsync();

        ViewBag.EventoId = new SelectList(
            eventos.Select(e => new { e.Id, Texto = $"{e.Titulo} · {e.Fecha:dd/MM/yyyy HH:mm}" }),
            "Id", "Texto", inscripcion.EventoId);

        ViewBag.AsistenteId = new SelectList(
            asistentes.Select(a => new { a.Id, Texto = $"{a.Nombres} · DNI {a.Dni}" }),
            "Id", "Texto", inscripcion.AsistenteId);
    }
}

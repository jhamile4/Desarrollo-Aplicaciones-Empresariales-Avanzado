using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.Web.Controllers;

public class AsistentesController : Controller
{
    private readonly AsistenteRepositorio _repositorio;

    public AsistentesController(AsistenteRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    // GET: /Asistentes
    public async Task<IActionResult> Index()
    {
        var asistentes = await _repositorio.ListarActivosAsync();
        return View(asistentes);
    }

    // GET: /Asistentes/Create
    public IActionResult Create()
    {
        return View(new Asistente());
    }

    // POST: /Asistentes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Asistente asistente)
    {
        if (!string.IsNullOrWhiteSpace(asistente.Dni) && await _repositorio.ExisteDniAsync(asistente.Dni))
            ModelState.AddModelError(nameof(Asistente.Dni), $"Ya existe un asistente con el DNI {asistente.Dni}.");

        if (!string.IsNullOrWhiteSpace(asistente.Correo) && await _repositorio.ExisteCorreoAsync(asistente.Correo))
            ModelState.AddModelError(nameof(Asistente.Correo), $"El correo {asistente.Correo} ya está registrado.");

        if (!ModelState.IsValid)
            return View(asistente);

        await _repositorio.CrearAsync(asistente);
        TempData["Exito"] = $"El asistente {asistente.Nombres} se registró correctamente.";
        return RedirectToAction(nameof(Index));
    }
}

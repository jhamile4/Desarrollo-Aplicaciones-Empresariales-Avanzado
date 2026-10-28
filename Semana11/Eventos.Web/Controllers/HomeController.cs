using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;

namespace Eventos.Web.Controllers;

public class HomeController : Controller
{
    private readonly EventoRepositorio _eventos;
    private readonly AsistenteRepositorio _asistentes;
    private readonly InscripcionRepositorio _inscripciones;

    public HomeController(EventoRepositorio eventos, AsistenteRepositorio asistentes, InscripcionRepositorio inscripciones)
    {
        _eventos = eventos;
        _asistentes = asistentes;
        _inscripciones = inscripciones;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Eventos = await _eventos.ContarActivosAsync();
        ViewBag.Asistentes = await _asistentes.ContarActivosAsync();
        ViewBag.Confirmadas = await _inscripciones.ContarPorEstadoAsync("Confirmada");
        ViewBag.Categorias = (await _eventos.ListarCategoriasActivasAsync()).Count;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

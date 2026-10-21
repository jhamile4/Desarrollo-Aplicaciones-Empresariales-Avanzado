using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Hotel.Web.Models;
using Hotel.Web.Repositorios;

namespace Hotel.Web.Controllers;

public class HomeController : Controller
{
    private readonly HabitacionRepositorio _habitaciones;
    private readonly ReservaRepositorio _reservas;

    public HomeController(HabitacionRepositorio habitaciones, ReservaRepositorio reservas)
    {
        _habitaciones = habitaciones;
        _reservas = reservas;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Habitaciones = await _habitaciones.ContarActivasAsync();
        ViewBag.Pendientes = await _reservas.ContarPorEstadoAsync("Pendiente");
        ViewBag.Confirmadas = await _reservas.ContarPorEstadoAsync("Confirmada");
        ViewBag.Tipos = (await _habitaciones.ListarTiposAsync()).Count;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

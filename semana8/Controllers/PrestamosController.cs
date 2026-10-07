using Biblioteca.Web.Models;
using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Web.Controllers
{
    public class PrestamosController : Controller
    {
        private readonly IPrestamoRepositorio _prestamoRepositorio;

        public PrestamosController(IPrestamoRepositorio prestamoRepositorio)
        {
            _prestamoRepositorio = prestamoRepositorio;
        }

        // GET: Prestamos/Reporte
        public async Task<IActionResult> Reporte(DateTime? desde, DateTime? hasta)
        {
            ViewData["Title"] = "Reporte de Préstamos por Intervalo de Fechas";
            ViewData["Desde"] = desde?.ToString("yyyy-MM-dd");
            ViewData["Hasta"] = hasta?.ToString("yyyy-MM-dd");

            var reporte = await _prestamoRepositorio.ReportePorFechasAsync(desde, hasta);
            return View(reporte);
        }
    }
}

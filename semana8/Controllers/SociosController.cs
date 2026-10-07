using Biblioteca.Web.Models;
using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Web.Controllers
{
    public class SociosController : Controller
    {
        private readonly ISocioRepositorio _socioRepositorio;

        public SociosController(ISocioRepositorio socioRepositorio)
        {
            _socioRepositorio = socioRepositorio;
        }

        // GET: Socios
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Listado de Socios Activos";
            var socios = await _socioRepositorio.ListarActivosAsync();
            return View(socios);
        }

        // GET: Socios/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Registrar Nuevo Socio";
            return View(new Socio());
        }

        // POST: Socios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Socio socio)
        {
            ViewData["Title"] = "Registrar Nuevo Socio";

            // Validar si el DNI ya existe en la base de datos antes de insertar
            if (!string.IsNullOrWhiteSpace(socio.DNI))
            {
                bool existeDNI = await _socioRepositorio.ExisteDNIAsync(socio.DNI.Trim());
                if (existeDNI)
                {
                    ModelState.AddModelError("DNI", $"El DNI '{socio.DNI}' ya se encuentra registrado por otro socio.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(socio);
            }

            await _socioRepositorio.InsertarAsync(socio);
            TempData["Mensaje"] = $"El socio '{socio.Nombre}' con DNI '{socio.DNI}' fue registrado exitosamente.";
            TempData["TipoMensaje"] = "success";

            return RedirectToAction(nameof(Index));
        }
    }
}

using Biblioteca.Web.Models;
using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Biblioteca.Web.Controllers
{
    public class LibrosController : Controller
    {
        private readonly ILibroRepositorio _libroRepositorio;

        public LibrosController(ILibroRepositorio libroRepositorio)
        {
            _libroRepositorio = libroRepositorio;
        }

        // GET: Libros
        public async Task<IActionResult> Index(string? titulo)
        {
            ViewData["Title"] = "Catálogo de Libros Activos";
            ViewData["TituloBusqueda"] = titulo;

            IEnumerable<Libro> libros;
            if (string.IsNullOrWhiteSpace(titulo))
            {
                libros = await _libroRepositorio.ListarActivosAsync();
            }
            else
            {
                libros = await _libroRepositorio.BuscarPorTituloAsync(titulo);
            }

            return View(libros);
        }

        // GET: Libros/Details/5
        public async Task<IActionResult> Details(int id)
        {
            ViewData["Title"] = "Detalle del Libro";

            var libro = await _libroRepositorio.ObtenerPorIdAsync(id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // GET: Libros/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Registrar Nuevo Libro";
            await CargarAutoresAsync();
            return View(new Libro());
        }

        // POST: Libros/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Registrar Nuevo Libro";
                await CargarAutoresAsync(libro.AutorId);
                return View(libro);
            }

            await _libroRepositorio.InsertarAsync(libro);
            TempData["Mensaje"] = $"El libro '{libro.Titulo}' ha sido registrado exitosamente.";
            TempData["TipoMensaje"] = "success";

            return RedirectToAction(nameof(Index));
        }

        // GET: Libros/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Editar Libro";

            var libro = await _libroRepositorio.ObtenerPorIdAsync(id);
            if (libro == null)
            {
                return NotFound();
            }

            await CargarAutoresAsync(libro.AutorId);
            return View(libro);
        }

        // POST: Libros/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Libro libro)
        {
            if (id != libro.LibroId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Editar Libro";
                await CargarAutoresAsync(libro.AutorId);
                return View(libro);
            }

            await _libroRepositorio.ActualizarAsync(libro);
            TempData["Mensaje"] = $"El libro '{libro.Titulo}' ha sido actualizado correctamente.";
            TempData["TipoMensaje"] = "info";

            return RedirectToAction(nameof(Index));
        }

        // GET: Libros/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            ViewData["Title"] = "Eliminar Libro";

            var libro = await _libroRepositorio.ObtenerPorIdAsync(id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // POST: Libros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libro = await _libroRepositorio.ObtenerPorIdAsync(id);
            await _libroRepositorio.EliminarLogicoAsync(id);

            TempData["Mensaje"] = $"El libro '{(libro != null ? libro.Titulo : "seleccionado")}' fue desactivado (eliminación lógica) correctamente.";
            TempData["TipoMensaje"] = "warning";

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarAutoresAsync(int? autorIdSeleccionado = null)
        {
            var autores = await _libroRepositorio.ListarAutoresActivosAsync();
            ViewData["Autores"] = new SelectList(autores, "AutorId", "Nombre", autorIdSeleccionado);
        }
    }
}

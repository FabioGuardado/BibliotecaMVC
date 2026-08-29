using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        private readonly IAutorService _autorService;

        public AutoresController(IAutorService autorService)
        {
            _autorService = autorService;
        }

        public IActionResult Index()
        {
            return View(_autorService.ObtenerTodos());
        }

        public IActionResult Details(int id)
        {
            var autor = _autorService.ObtenerAutorPorId(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Autor autor)
        {
            if (ModelState.IsValid)
            {
                autor.ID = _autorService.ObtenerIdParaNuevoAutor();
                _autorService.Crear(autor);
                return RedirectToAction("Index");
            }
            return View(autor);
        }

        public IActionResult Edit(int id)
        {
            var autor = _autorService.ObtenerAutorPorId(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Autor autor)
        {
            if (ModelState.IsValid)
            {
                var existingAutor = _autorService.ObtenerAutorPorId(autor.ID);
                if (existingAutor == null)
                {
                    return NotFound();
                }

                existingAutor.Nombre = autor.Nombre;
                existingAutor.Apellido = autor.Apellido;
                existingAutor.Nacionalidad = autor.Nacionalidad;
                existingAutor.FechaNacimiento = autor.FechaNacimiento;
                existingAutor.Activo = autor.Activo;

                return RedirectToAction("Index");
            }
            return View(autor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var item = _autorService.ObtenerAutorPorId(id);
            if (item != null)
            {
                _autorService.Eliminar(item);
            }

            return RedirectToAction("Index");
        }
    }
}

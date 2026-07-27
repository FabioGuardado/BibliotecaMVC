using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private static List<Libro> _libros = new List<Libro>()
            {
                new Libro { ID = 1, Titulo = "Clean Code", Autor = "Robert Martin", Categoria = "Programación", Precio = 35.5M, Disponible = true },
                new Libro { ID = 2, Titulo = "Cien Años de Soledad", Autor = "Gabriel García Márquez", Categoria = "Literatura", Precio = 18, Disponible = false },
            };

        public IActionResult Index()
        {
            return View(_libros);
        }

        public IActionResult Details(int id)
        {
            var libro = _libros.FirstOrDefault(a => a.ID == id);
            if (libro == null)
            {
                return NotFound();
            }
            return View(libro);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Libro libro)
        {
            if (ModelState.IsValid)
            {
                libro.ID = _libros.Max(a => a.ID) + 1;
                _libros.Add(libro);
                return RedirectToAction("Index");
            }
            return View(libro);
        }

        public IActionResult Edit(int id)
        {
            var libro = _libros.FirstOrDefault(a => a.ID == id);
            if (libro == null)
            {
                return NotFound();
            }
            return View(libro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Libro libro)
        {
            if (ModelState.IsValid)
            {
                var existinglibro = _libros.FirstOrDefault(a => a.ID == libro.ID);
                if (existinglibro == null)
                {
                    return NotFound();
                }

                existinglibro.Titulo = libro.Titulo;
                existinglibro.Autor = libro.Autor;
                existinglibro.Categoria = libro.Categoria;
                existinglibro.Precio = libro.Precio;
                existinglibro.Disponible = libro.Disponible;

                return RedirectToAction("Index");
            }
            return BadRequest();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var item = _libros.FirstOrDefault(a => a.ID == id);
            if (item != null)
            {
                _libros.Remove(item);
            }

            return RedirectToAction("Index");
        }
    }
}

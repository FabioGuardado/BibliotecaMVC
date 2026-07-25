using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        private static List<Autor> _autores = new List<Autor>
            {
                new Autor { ID = 1, Nombre = "Gabriel", Apellido = "García Márquez", Nacionalidad = "Colombia", FechaNacimiento = new DateTime(1927, 3, 6), Activo = false },
                new Autor { ID = 2, Nombre = "Robert", Apellido = "Martin", Nacionalidad = "Estados Unidos", FechaNacimiento = new DateTime(1952, 12, 5), Activo = true },
                new Autor { ID = 3, Nombre = "Isabel", Apellido = "Allende", Nacionalidad = "Chile", FechaNacimiento = new DateTime(1942, 8, 2), Activo = true },
                new Autor { ID = 4, Nombre = "Claudia", Apellido = "Lars", Nacionalidad = "El Salvador", FechaNacimiento = new DateTime(1899, 12, 20), Activo = false },
                new Autor { ID = 5, Nombre = "Stephen", Apellido = "King", Nacionalidad = "Estados Unidos", FechaNacimiento = new DateTime(1947, 9, 21), Activo = true }
            };
        public IActionResult Index()
        {
            // ViewBag.Autores = _autores;

            return View(_autores);
        }

        public IActionResult Details(int id)
        {
            var autor = _autores.FirstOrDefault(a => a.ID == id);
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
                autor.ID = _autores.Max(a => a.ID) + 1;
                _autores.Add(autor);
                return RedirectToAction(nameof(Index));
            }
            return View(autor);
        }
    }
}

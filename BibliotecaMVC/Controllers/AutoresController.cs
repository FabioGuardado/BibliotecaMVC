using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        public IActionResult Index()
        {
            List<Autor> autores = new List<Autor>
            {
                new Autor { ID = 1, Nombre = "Gabriel", Apellido = "García Márquez", Nacionalidad = "Colombia", FechaNacimiento = new DateTime(1927, 3, 6), Activo = false },
                new Autor { ID = 2, Nombre = "Robert", Apellido = "Martin", Nacionalidad = "Estados Unidos", FechaNacimiento = new DateTime(1952, 12, 5), Activo = true },
                new Autor { ID = 3, Nombre = "Isabel", Apellido = "Allende", Nacionalidad = "Chile", FechaNacimiento = new DateTime(1942, 8, 2), Activo = true },
                new Autor { ID = 4, Nombre = "Claudia", Apellido = "Lars", Nacionalidad = "El Salvador", FechaNacimiento = new DateTime(1899, 12, 20), Activo = false },
                new Autor { ID = 5, Nombre = "Stephen", Apellido = "King", Nacionalidad = "Estados Unidos", FechaNacimiento = new DateTime(1947, 9, 21), Activo = true }
            };

            ViewBag.Autores = autores;

            return View();
        }
    }
}

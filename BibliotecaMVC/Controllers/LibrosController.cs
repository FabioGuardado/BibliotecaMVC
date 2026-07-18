using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        public IActionResult Index()
        {
            List<Libro> libros = new List<Libro>()
            {
                new Libro { ID = 1, Titulo = "Clean Code", Autor = "Robert Martin", Categoria = "Programación", Precio = 35.5M, Disponible = true },
                new Libro { ID = 2, Titulo = "Cien Años de Soledad", Autor = "Gabriel García Márquez", Categoria = "Literatura", Precio = 18, Disponible = false },
            };

            ViewBag.Nombre = "Fabio Guardado";
            ViewBag.Libros = libros;

            return View();
        }
    }
}

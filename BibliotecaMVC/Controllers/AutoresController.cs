using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        private readonly IAutorService _autorService;

        private readonly BibliotecaContext _context;

        public AutoresController(IAutorService autorService, BibliotecaContext context)
        {
            _autorService = autorService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // return View(_autorService.ObtenerTodos());

            var autores = await _context.Autores.ToListAsync();
            return View(autores);
        }

        public async Task<IActionResult> Details(int id)
        {
            // var autor = _autorService.ObtenerAutorPorId(id);

            var autor = await _context.Autores.FindAsync(id);

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
        public async Task<IActionResult> Create(Autor autor)
        {
            /* if (ModelState.IsValid)
            {
                autor.ID = _autorService.ObtenerIdParaNuevoAutor();
                _autorService.Crear(autor);
                return RedirectToAction("Index");
            }
            return View(autor); */

            if (!ModelState.IsValid)
            {
                return View(autor);
            }

            _context.Autores.Add(autor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            // var autor = _autorService.ObtenerAutorPorId(id);
            var autor = await _context.Autores.FindAsync(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Autor autor)
        {
            //if (ModelState.IsValid)
            //{
            //    var existingAutor = _autorService.ObtenerAutorPorId(autor.ID);
            //    if (existingAutor == null)
            //    {
            //        return NotFound();
            //    }

            //    existingAutor.Nombre = autor.Nombre;
            //    existingAutor.Apellido = autor.Apellido;
            //    existingAutor.Nacionalidad = autor.Nacionalidad;
            //    existingAutor.FechaNacimiento = autor.FechaNacimiento;
            //    existingAutor.Activo = autor.Activo;

            //    return RedirectToAction("Index");
            //}
            //return View(autor);

            if (!ModelState.IsValid)
            {
                return View(autor);
            }

            var exists = await _context.Autores.AnyAsync(a => a.ID == autor.ID);

            if (!exists) 
            { 
                return NotFound(); 
            }

            _context.Update(autor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            //var item = _autorService.ObtenerAutorPorId(id);
            //if (item != null)
            //{
            //    _autorService.Eliminar(item);
            //}

            //return RedirectToAction("Index");

            var autor = await _context.Autores.FindAsync(id);
            if (autor == null)
            {
                return NotFound();
            }

            _context.Autores.Remove(autor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

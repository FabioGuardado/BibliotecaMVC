using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class AutoresController : Controller
    {
        private readonly IAutorService _autorService;

        private readonly BibliotecaContext _context;
        private readonly ILogger<AutoresController> _logger;

        public AutoresController(IAutorService autorService, BibliotecaContext context, ILogger<AutoresController> logger)
        {
            _autorService = autorService;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // return View(_autorService.ObtenerTodos());

            _logger.LogInformation("Consultando el listado de Autores");

            try
            {
                var autores = await _context.Autores.ToListAsync();
                return View(autores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar el listado de autores");
                throw;
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            // var autor = _autorService.ObtenerAutorPorId(id);

            _logger.LogInformation("Consultando los detalles del autor con identificador {AutorId}", id);
            try
            {
                var autor = await _context.Autores.FindAsync(id);

                if (autor == null)
                {
                    _logger.LogWarning("No se encontró el autor solicitado con identificador {AutorId}", id);
                    return NotFound();
                }
                return View(autor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar los detalles del autor con identificador {AutorId}", id);
                throw;
            }
        }

        [AuthorizeRoles(Roles.Admin, Roles.Bibliotecario)]
        public IActionResult Create()
        {
            _logger.LogInformation("Iniciando la operación de creación de un autor");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Roles.Admin, Roles.Bibliotecario)]
        public async Task<IActionResult> Create(Autor autor)
        {
            /* if (ModelState.IsValid)
            {
                autor.ID = _autorService.ObtenerIdParaNuevoAutor();
                _autorService.Crear(autor);
                return RedirectToAction("Index");
            }
            return View(autor); */

            _logger.LogInformation("Iniciando la operación de creación de un autor");
            if (!ModelState.IsValid)
            {
                _logger.LogError("El autor enviado no cumple con los requerimientos mínimos.");
                return View(autor);
            }

            try
            {
                _context.Autores.Add(autor);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al crear el autor");
                throw;
            }
        }

        [AuthorizeRoles(Roles.Admin, Roles.Bibliotecario)]
        public async Task<IActionResult> Edit(int id)
        {
            // var autor = _autorService.ObtenerAutorPorId(id);

            _logger.LogInformation("Iniciando la operación de edición del autor con identificador {AutorId}", id);

            try
            {
                var autor = await _context.Autores.FindAsync(id);
                if (autor == null)
                {
                    return NotFound();
                }
                return View(autor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar los detalles del autor para su edición con identificador {AutorId}", id);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Roles.Admin, Roles.Bibliotecario)]
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

            _logger.LogInformation("Iniciando la edición del autor con identificador {AutorId}", autor.ID);

            if (!ModelState.IsValid)
            {
                return View(autor);
            }

            try
            {
                var exists = await _context.Autores.AnyAsync(a => a.ID == autor.ID);

                if (!exists)
                {
                    _logger.LogWarning("No se encontró el autor solicitado con identificador {AutorId}", autor.ID);
                    return NotFound();
                }

                _context.Update(autor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al editar el autor con identificador {AutorId}", autor.ID);
                throw;
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            //var item = _autorService.ObtenerAutorPorId(id);
            //if (item != null)
            //{
            //    _autorService.Eliminar(item);
            //}

            //return RedirectToAction("Index");

            _logger.LogInformation("Iniciando la eliminación del autor con identificador {AutorId}", id);

            try
            {
                var autor = await _context.Autores.FindAsync(id);
                if (autor == null)
                {
                    _logger.LogWarning("No se encontró el autor solicitado con identificador {AutorId}", id);
                    return NotFound();
                }

                _context.Autores.Remove(autor);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al eliminar el autor con identificador {AutorId}", id);
                throw;
            }

        }
    }
}

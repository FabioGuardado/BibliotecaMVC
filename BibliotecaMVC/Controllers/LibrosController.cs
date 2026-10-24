using BibliotecaMVC.Authorization;
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
    public class LibrosController : Controller
    {
        private readonly IRepositorioLibro _repositorio;

        private readonly IWebHostEnvironment _environment;

        private readonly BibliotecaContext _context;
        private readonly ILogger<AutoresController> _logger;

        public LibrosController(IWebHostEnvironment environment, IRepositorioLibro repositorioLibro, BibliotecaContext context, ILogger<AutoresController> logger)
        {
            _environment = environment;
            _repositorio = repositorioLibro;
            _context = context;
            _logger = logger;
        }

        private async Task<string> ProcessImageUploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            string fileExtension = Path.GetExtension(file.FileName);
            string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

            string folderPath = Path.Combine(_environment.WebRootPath, "Images");
            string physicalPath = Path.Combine(folderPath, uniqueFileName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            using (var fileStream = new FileStream(physicalPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/Images/" + uniqueFileName;
        }


        public async Task<IActionResult> Index()
        {
            // var libros = _repositorio.ObtenerTodos();

            _logger.LogInformation("Consultando el listado de Libros");

            try
            {
                var libros = await _context.Libros.ToListAsync();
                return View(libros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar el listado de libros");
                throw;
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            // var libro = _repositorio.ObtenerLibroPorId(id);

            _logger.LogInformation("Consultando los detalles del libro con identificador {LibroId}", id);
            
            try
            {
                var libro = await _context.Libros.FindAsync(id);
                if (libro == null)
                {
                    _logger.LogWarning("No se encontró el libro solicitado con identificador {LibroId}", id);
                    return NotFound();
                }
                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar los detalles del libro con identificador {LibroId}", id);
                throw;
            }
        }

        [AuthorizeRoles(Roles.Admin, Roles.Bibliotecario)]
        public IActionResult Create()
        {
            _logger.LogInformation("Iniciando la operación de creación de un libro");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Roles.Admin, Roles.Bibliotecario)]
        public async Task<IActionResult> Create(CreateLibroViewModel libro)
        {
            /* if (ModelState.IsValid)
            {
                string imageUrl = await ProcessImageUploadAsync(libro.ImageFile);

                var newLibro = new Libro()
                {
                    ID = _repositorio.ObtenerIdParaNuevoLibro(),
                    Autor = libro.Autor,
                    Titulo = libro.Titulo,
                    Categoria = libro.Categoria,
                    Precio = libro.Precio,
                    Disponible = libro.Disponible ?? true,
                    ImageUrl = imageUrl
                };

               _repositorio.Crear(newLibro);
                return RedirectToAction("Index");
            }
            return View(libro); */

            _logger.LogInformation("Iniciando la operación de creación de un libro");

            if (!ModelState.IsValid)
            {
                _logger.LogError("El libro enviado no cumple con los requerimientos mínimos.");
                return View(libro);
            }

            try
            {
                string imageUrl = await ProcessImageUploadAsync(libro.ImageFile);

                var newLibro = new Libro()
                {
                    Autor = libro.Autor,
                    Titulo = libro.Titulo,
                    Categoria = libro.Categoria,
                    Precio = libro.Precio,
                    Disponible = libro.Disponible ?? true,
                    ImageUrl = imageUrl
                };

                _context.Libros.Add(newLibro);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al crear el libro");
                throw;
            }
        }

        [AuthorizeRoles(Roles.Admin, Roles.Bibliotecario)]
        public async Task<IActionResult> Edit(int id)
        {
            // var libro = _repositorio.ObtenerLibroPorId(id);
            _logger.LogInformation("Iniciando la operación de edición del libro con identificador {LibroId}", id);

            try
            {
                var libro = await _context.Libros.FindAsync(id);
                if (libro == null)
                {
                    return NotFound();
                }
                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar los detalles del libro para su edición con identificador {LibroId}", id);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Roles.Admin, Roles.Bibliotecario)]
        public async Task<IActionResult> Edit(Libro libro)
        {
            //if (ModelState.IsValid)
            //{
            //    var existinglibro = _repositorio.ObtenerLibroPorId(libro.ID);
            //    if (existinglibro == null)
            //    {
            //        return NotFound();
            //    }

            //    existinglibro.Titulo = libro.Titulo;
            //    existinglibro.Autor = libro.Autor;
            //    existinglibro.Categoria = libro.Categoria;
            //    existinglibro.Precio = libro.Precio;
            //    existinglibro.Disponible = libro.Disponible;

            //    return RedirectToAction("Index");
            //}
            //return View(libro);

            _logger.LogInformation("Iniciando la edición del libro con identificador {LibroID}", libro.ID);

            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            try
            {
                var exists = await _context.Libros.AnyAsync(a => a.ID == libro.ID);

                if (!exists)
                {
                    return NotFound();
                }

                _context.Update(libro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al editar el libro con identificador {LibroID}", libro.ID);
                throw;
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            //var item = _repositorio.ObtenerLibroPorId(id);
            //if (item != null)
            //{
            //    _repositorio.Eliminar(item);
            //}

            //return RedirectToAction("Index");

            _logger.LogInformation("Iniciando la eliminación del autor con identificador {LibroID}", id);

            try
            {
                var libro = await _context.Libros.FindAsync(id);
                if (libro == null)
                {
                    _logger.LogWarning("No se encontró el libro solicitado con identificador {LibroID}", id);
                    return NotFound();
                }

                _context.Libros.Remove(libro);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al eliminar el libro con identificador {LibroID}", id);
                throw;
            }
        }
    }
}

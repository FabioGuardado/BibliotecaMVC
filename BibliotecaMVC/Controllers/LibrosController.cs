using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private readonly IRepositorioLibro _repositorio;

        private readonly IWebHostEnvironment _environment;

        public LibrosController(IWebHostEnvironment environment, IRepositorioLibro repositorioLibro)
        {
            _environment = environment;
            _repositorio = repositorioLibro;
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


        public IActionResult Index()
        {
            var libros = _repositorio.ObtenerTodos();
            return View(libros);
        }

        public IActionResult Details(int id)
        {
            var libro = _repositorio.ObtenerLibroPorId(id);
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
        public async Task<IActionResult> Create(CreateLibroViewModel libro)
        {
            if (ModelState.IsValid)
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
            return View(libro);
        }

        public IActionResult Edit(int id)
        {
            var libro = _repositorio.ObtenerLibroPorId(id);
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
                var existinglibro = _repositorio.ObtenerLibroPorId(libro.ID);
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
            return View(libro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var item = _repositorio.ObtenerLibroPorId(id);
            if (item != null)
            {
                _repositorio.Eliminar(item);
            }

            return RedirectToAction("Index");
        }
    }
}

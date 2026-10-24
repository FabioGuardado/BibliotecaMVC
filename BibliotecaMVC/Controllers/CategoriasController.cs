using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class CategoriasController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<AutoresController> _logger;

        public CategoriasController(IConfiguration configuration, ILogger<AutoresController> logger)
        {
            _connectionString = configuration.GetConnectionString("BibliotecaDB");
            _logger = logger;
        }
        public IActionResult Index()
        {

            _logger.LogInformation("Consultando el listado de categorias");

            try
            {
                var categorias = new List<Categoria>();

                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "SELECT ID, Nombre, Descripcion FROM Categorias";
                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        conexion.Open();
                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                categorias.Add(new Categoria
                                {
                                    ID = lector.GetInt32(0),
                                    Nombre = lector.GetString(1),
                                    Descripcion = lector.IsDBNull(2) ? null : lector.GetString(2)
                                });
                            }
                        }
                    }
                    return View(categorias);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar el listado de categorias");
                throw;
            }
        }

        [AuthorizeRoles(Roles.Admin, Roles.Bibliotecario)]
        public IActionResult Create()
        {

            _logger.LogInformation("Iniciando la operación de creación de una categoria");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Roles.Admin, Roles.Bibliotecario)]
        public IActionResult Create(Categoria categoria)
        {
            _logger.LogInformation("Iniciando la operación de creación de una categoria");
            if (categoria == null || string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                ModelState.AddModelError("Nombre", "El nombre es obligatorio");
                return View(categoria);
            }

            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "INSERT INTO Categorias (Nombre, Descripcion) VALUES (@Nombre, @Descripcion)";
                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                        comando.Parameters.AddWithValue("@Descripcion", (object)categoria.Descripcion ?? System.DBNull.Value);

                        conexion.Open();
                        comando.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "Categoria guardada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al crear la categoria");
                throw;
            }
        }

        [AuthorizeRoles(Roles.Admin, Roles.Bibliotecario)]
        public IActionResult Edit(int id)
        {
            _logger.LogInformation("Iniciando la operación de edición de la categoria con identificador {CategoriaId}", id);

            try
            {
                Categoria categoria = null;

                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "SELECT ID, Nombre, Descripcion FROM Categorias WHERE ID = @ID";
                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@ID", id);

                        conexion.Open();
                        using (var lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                categoria = new Categoria
                                {
                                    ID = lector.GetInt32(0),
                                    Nombre = lector.GetString(1),
                                    Descripcion = lector.IsDBNull(2) ? null : lector.GetString(2)
                                };
                            }
                        }
                    }
                }

                if (categoria == null)
                {
                    return NotFound();
                }

                return View(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar los detalles de la categoria para su edición con identificador {CategoriaId}", id);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Roles.Admin, Roles.Bibliotecario)]
        public IActionResult Edit(Categoria categoria)
        {

            _logger.LogInformation("Iniciando la edición de la categoria con identificador {CategoriaId}", categoria.ID);

            if (categoria == null || string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                ModelState.AddModelError("Nombre", "El nombre es obligatorio");
                return View(categoria);
            }

            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "UPDATE Categorias SET Nombre = @Nombre, Descripcion = @Descripcion WHERE ID = @ID";
                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                        comando.Parameters.AddWithValue("@Descripcion", (object)categoria.Descripcion ?? System.DBNull.Value);
                        comando.Parameters.AddWithValue("@ID", categoria.ID);

                        conexion.Open();
                        comando.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "Categoria actualizada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al editar la categoria con identificador {CategoriaId}", categoria.ID);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Roles.Admin)]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation("Iniciando la eliminación de la categoria con identificador {CategoriaId}", id);

            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "DELETE FROM Categorias WHERE ID = @ID";
                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@ID", id);

                        conexion.Open();
                        comando.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "Categoria eliminada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al eliminar la categoria con identificador {CategoriaId}", id);
                throw;
            }
        }
    }
}

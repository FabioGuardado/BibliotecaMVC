using BibliotecaMVC.Models;

namespace BibliotecaMVC.Repositories
{
    public class RepositorioEnMemoria : IRepositorioLibro
    {
        private List<Libro> _libros = new List<Libro>()
        {
                new Libro { ID = 1, Titulo = "Clean Code", Autor = "Robert Martin", Categoria = "Programación", Precio = 35.5M, Disponible = true, ImageUrl = null },
                new Libro { ID = 2, Titulo = "Cien Años de Soledad", Autor = "Gabriel García Márquez", Categoria = "Literatura", Precio = 18, Disponible = false, ImageUrl = null },
            };
        public IEnumerable<Libro> ObtenerTodos()
        {
            return _libros;
            
        }

        public Libro? ObtenerLibroPorId(int Id)
        {
            var libro = _libros.FirstOrDefault(a => a.ID == Id);
            return libro;
        }

        public int ObtenerIdParaNuevoLibro() {
            return _libros.Max(a => a.ID) + 1;
        }

        public Libro Crear(Libro libro)
        {
            _libros.Add(libro);
            return libro;
        }

        public void Eliminar(Libro libro)
        {
            _libros.Remove(libro);
        }
    }
}

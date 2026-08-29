using BibliotecaMVC.Models;

namespace BibliotecaMVC.Repositories
{
    public interface IRepositorioLibro
    {
        IEnumerable<Libro> ObtenerTodos();

        Libro? ObtenerLibroPorId(int Id);

        Libro Crear(Libro libro);

        void Eliminar(Libro libro);

        int ObtenerIdParaNuevoLibro();
    }
}

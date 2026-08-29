using BibliotecaMVC.Models;

namespace BibliotecaMVC.Repositories
{
    public interface IAutorService
    {
        IEnumerable<Autor> ObtenerTodos();

        Autor? ObtenerAutorPorId(int Id);

        Autor Crear(Autor autor);

        void Eliminar(Autor autor);

        int ObtenerIdParaNuevoAutor();
    }
}

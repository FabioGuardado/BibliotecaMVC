using BibliotecaMVC.Models;

namespace BibliotecaMVC.Repositories
{
    public class AutorService : IAutorService
    {
        private static List<Autor> _autores = new List<Autor>
            {
                new Autor { ID = 1, Nombre = "Gabriel", Apellido = "García Márquez", Nacionalidad = "Colombia", FechaNacimiento = new DateTime(1927, 3, 6), Activo = false },
                new Autor { ID = 2, Nombre = "Robert", Apellido = "Martin", Nacionalidad = "Estados Unidos", FechaNacimiento = new DateTime(1952, 12, 5), Activo = true },
                new Autor { ID = 3, Nombre = "Isabel", Apellido = "Allende", Nacionalidad = "Chile", FechaNacimiento = new DateTime(1942, 8, 2), Activo = true },
                new Autor { ID = 4, Nombre = "Claudia", Apellido = "Lars", Nacionalidad = "El Salvador", FechaNacimiento = new DateTime(1899, 12, 20), Activo = false },
                new Autor { ID = 5, Nombre = "Stephen", Apellido = "King", Nacionalidad = "Estados Unidos", FechaNacimiento = new DateTime(1947, 9, 21), Activo = true }
            };

        public IEnumerable<Autor> ObtenerTodos()
        {
            return _autores;
        }

        public Autor? ObtenerAutorPorId(int Id)
        {
            return _autores.FirstOrDefault(a => a.ID == Id);
        }

        public int ObtenerIdParaNuevoAutor()
        {
            return _autores.Max(a => a.ID) + 1;
        }

        public Autor Crear(Autor autor)
        {
            _autores.Add(autor);
            return autor;
        }

        public void Eliminar(Autor autor)
        {
            _autores.Remove(autor);
        }
    }
}

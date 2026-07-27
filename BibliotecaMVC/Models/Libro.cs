using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class Libro
    {
        public int ID { get; set; }
        [Required]
        public string Titulo { get; set; }
        [Required]
        public string Autor { get; set; }
        [Required]
        public string Categoria { get; set; }
        [Required]
        public decimal Precio { get; set; }
        public bool Disponible { get; set; } = true;
    }
}

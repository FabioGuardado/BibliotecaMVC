using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class CreateLibroViewModel
    {
        [Required]
        public string Titulo { get; set; }
        [Required]
        public string Autor { get; set; }
        [Required]
        public string Categoria { get; set; }
        [Required]
        public decimal Precio { get; set; }
        public bool? Disponible { get; set; } = null;
        public IFormFile ImageFile { get; set; }
    }
}

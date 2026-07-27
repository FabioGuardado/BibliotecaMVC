using System;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class Autor
    {
        public int ID { get; set; }
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        
        [StringLength(50)]
        public string Nacionalidad { get; set; }
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }
        public bool Activo { get; set; } = true;
    }
}

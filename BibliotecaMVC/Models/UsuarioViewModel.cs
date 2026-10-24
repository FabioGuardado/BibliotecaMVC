using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class UsuarioViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Display(Name = "Usuario")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Rol")]
        public string Rol { get; set; } = string.Empty;
    }

    public class CreateUsuarioViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [Display(Name = "Usuario")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Introduce un correo electrónico válido")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmación de contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmación de contraseña")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio")]
        [Display(Name = "Rol")]
        public string Rol { get; set; } = nameof(Authorization.Roles.Usuario);
    }

    public class EditUsuarioViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [Display(Name = "Usuario")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Introduce un correo electrónico válido")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio")]
        [Display(Name = "Rol")]
        public string Rol { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña (opcional)")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmación de nueva contraseña")]
        public string? ConfirmNewPassword { get; set; }
    }
}

using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Controllers
{
    [AuthorizeRoles(Roles.Admin)]
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UsuariosController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        private static List<SelectListItem> ObtenerRoles(string? seleccionado = null) =>
            Enum.GetNames<Roles>()
                .Select(r => new SelectListItem(r, r, r == seleccionado))
                .ToList();

        private async Task<UsuarioViewModel> ToViewModelAsync(IdentityUser user) => new()
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Rol = string.Join(", ", await _userManager.GetRolesAsync(user))
        };

        public async Task<IActionResult> Index()
        {
            var usuarios = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();

            var modelo = new List<UsuarioViewModel>();
            foreach (var usuario in usuarios)
            {
                modelo.Add(await ToViewModelAsync(usuario));
            }

            return View(modelo);
        }

        public async Task<IActionResult> Details(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(await ToViewModelAsync(usuario));
        }

        public IActionResult Create()
        {
            var modelo = new CreateUsuarioViewModel();
            ViewBag.Roles = ObtenerRoles(modelo.Rol);
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUsuarioViewModel model)
        {
            ViewBag.Roles = ObtenerRoles(model.Rol);

            if (!Enum.TryParse<Roles>(model.Rol, out _))
            {
                ModelState.AddModelError(nameof(model.Rol), "El rol seleccionado no es válido");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(usuario, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            var roleResult = await _userManager.AddToRoleAsync(usuario, model.Rol);
            if (!roleResult.Succeeded)
            {
                // Evita dejar un usuario sin rol si falla la asignación
                await _userManager.DeleteAsync(usuario);
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var rol = (await _userManager.GetRolesAsync(usuario)).FirstOrDefault() ?? nameof(Roles.Usuario);

            ViewBag.Roles = ObtenerRoles(rol);
            return View(new EditUsuarioViewModel
            {
                Id = usuario.Id,
                UserName = usuario.UserName ?? string.Empty,
                Email = usuario.Email ?? string.Empty,
                Rol = rol
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUsuarioViewModel model)
        {
            ViewBag.Roles = ObtenerRoles(model.Rol);

            if (!Enum.TryParse<Roles>(model.Rol, out _))
            {
                ModelState.AddModelError(nameof(model.Rol), "El rol seleccionado no es válido");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = await _userManager.FindByIdAsync(model.Id);
            if (usuario == null)
            {
                return NotFound();
            }

            var rolesActuales = await _userManager.GetRolesAsync(usuario);

            // Un admin no puede quitarse el rol Admin a sí mismo (evita quedarse sin acceso)
            if (usuario.Id == _userManager.GetUserId(User) && model.Rol != nameof(Roles.Admin))
            {
                ModelState.AddModelError(nameof(model.Rol), "No puedes quitarte el rol Admin a ti mismo");
                return View(model);
            }

            usuario.UserName = model.UserName;
            usuario.Email = model.Email;

            var result = await _userManager.UpdateAsync(usuario);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            if (!rolesActuales.Contains(model.Rol) || rolesActuales.Count != 1)
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
                var addResult = removeResult.Succeeded
                    ? await _userManager.AddToRoleAsync(usuario, model.Rol)
                    : removeResult;

                if (!addResult.Succeeded)
                {
                    foreach (var error in addResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
                var passwordResult = await _userManager.ResetPasswordAsync(usuario, token, model.NewPassword);
                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                    {
                        ModelState.AddModelError(nameof(model.NewPassword), error.Description);
                    }
                    return View(model);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            if (usuario.Id == _userManager.GetUserId(User))
            {
                TempData["Error"] = "No puedes eliminar tu propia cuenta.";
                return RedirectToAction(nameof(Index));
            }

            await _userManager.DeleteAsync(usuario);
            return RedirectToAction(nameof(Index));
        }
    }
}

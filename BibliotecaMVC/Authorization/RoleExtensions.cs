using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace BibliotecaMVC.Authorization
{

    public static class RoleExtensions
    {
        public static Task<IdentityResult> AddToRoleAsync(
            this UserManager<IdentityUser> um, IdentityUser user, Roles rol)
            => um.AddToRoleAsync(user, rol.ToString());

        public static Task<IdentityResult> RemoveFromRoleAsync(
            this UserManager<IdentityUser> um, IdentityUser user, Roles rol)
            => um.RemoveFromRoleAsync(user, rol.ToString());

        public static Task<bool> IsInRoleAsync(
            this UserManager<IdentityUser> um, IdentityUser user, Roles rol)
            => um.IsInRoleAsync(user, rol.ToString());

        public static bool IsInRole(this ClaimsPrincipal user, Roles rol)
            => user.IsInRole(rol.ToString());

        public static async Task<List<Roles>> GetRolesEnumAsync(
            this UserManager<IdentityUser> um, IdentityUser user)
        {
            var nombres = await um.GetRolesAsync(user);
            return nombres
                .Select(n => Enum.TryParse<Roles>(n, out var r) ? (Roles?)r : null)
                .Where(r => r.HasValue)
                .Select(r => r!.Value)
                .ToList();
        }
    }
}

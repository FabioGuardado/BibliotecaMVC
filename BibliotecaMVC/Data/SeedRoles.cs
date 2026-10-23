using Microsoft.AspNetCore.Identity;

namespace BibliotecaMVC.Data
{
    public class SeedRoles
    {
        public static async Task InicializarAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            foreach (var rol in Enum.GetValues<Roles>())
            {
                var nombre = rol.ToString();
                if (!await roleManager.RoleExistsAsync(nombre))
                    await roleManager.CreateAsync(new IdentityRole(nombre));
            }

            var adminEmail = "admin@miapp.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, "Admin123!");
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}

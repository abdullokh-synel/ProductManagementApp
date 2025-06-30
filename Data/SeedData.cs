using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductManagementApp.Models;

namespace ProductManagementApp.Data
{
    public class SeedData()
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await context.Database.MigrateAsync();

            var roles = new[] { "admin", "user" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            var adminEmail = "admin@test.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "admin");
                }
                else
                {
                    throw new Exception("Error while adding admin: " + string.Join("; ", result.Errors.Select(e => e.Description)));
                }
            }

            var userEmail = "user@test.com";
            var normalUser = await userManager.FindByEmailAsync(userEmail);
            if (normalUser == null)
            {
                normalUser = new ApplicationUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(normalUser, "User123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(normalUser, "user");
                }
                else
                {
                    throw new Exception("Error while adding user: " + string.Join("; ", result.Errors.Select(e => e.Description)));
                }
            }

            if (!context.Products.Any())
            {
                context.Products.AddRange(
                    new Product { Title = "HDD 1TB", Quantity = 55, Price = 74.09M },
                    new Product { Title = "HDD SSD 512GB", Quantity = 102, Price = 190.99M },
                    new Product { Title = "RAM DDR4 16GB", Quantity = 47, Price = 80.32M }
                );
                await context.SaveChangesAsync();
            }
        }

    }
}
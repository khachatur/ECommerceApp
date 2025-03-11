using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ECommerceApp.Infrastructure;

public static class DatabaseSeeder
{
    // Seed categories and products
    public static async Task SeedAsync(IAppDbContext context)
    {
        if (!context.Categories.Any())
        {
            context.Categories.Add(new Category { Name = "Electronics" });
            context.Categories.Add(new Category { Name = "Books" });
            await context.SaveChangesAsync(CancellationToken.None);
        }

        if (!context.Products.Any())
        {
            context.Products.Add(new Product
            {
                Name = "Laptop",
                Description = "A powerful laptop",
                Price = 999.99m,
                Quantity = 10,
                CategoryId = 1
            });
            context.Products.Add(new Product
            {
                Name = "Novel",
                Description = "A bestselling novel",
                Price = 19.99m,
                Quantity = 50,
                CategoryId = 2
            });
            await context.SaveChangesAsync(CancellationToken.None);
        }
    }

    // Seed users and roles
    public static async Task SeedUsersAsync(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Seed roles
        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Seed admin user
        if (await userManager.FindByNameAsync("admin") == null)
        {
            var admin = new IdentityUser { UserName = "admin@example.com", Email = "admin@example.com" };
            await userManager.CreateAsync(admin, "AdminPassword123!");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        // Seed regular user
        if (await userManager.FindByNameAsync("user") == null)
        {
            var user = new IdentityUser { UserName = "\"user@example.com", Email = "user@example.com" };
            await userManager.CreateAsync(user, "UserPassword123!");
            await userManager.AddToRoleAsync(user, "User");
        }
    }
}

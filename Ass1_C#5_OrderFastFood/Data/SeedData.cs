using Ass1_C_5_OrderFastFood.Models;
using Microsoft.AspNetCore.Identity;

namespace Ass1_C_5_OrderFastFood.Data
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] roles = new[] { "Admin", "Customer", "Guest" };

            foreach (var r in roles)
            {
                if (!await roleManager.RoleExistsAsync(r))
                    await roleManager.CreateAsync(new IdentityRole(r));
            }

            var adminEmail = "admin@gmail.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    FullName = "Admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                };
                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                    await userManager.AddClaimAsync(admin, new System.Security.Claims.Claim("Permission", "ManageUsers"));
                }
            }

            // optional: seed categories, fooditems, combos etc using ApplicationDbContext
            var db = serviceProvider.GetRequiredService<ApplicationDbContext>();
            if (!db.FoodCategories.Any())
            {
                var cat = new FoodCategory { Name = "Burgers" };
                db.FoodCategories.Add(cat);
                db.FoodItems.Add(new FoodItem { Name = "Classic Burger", Price = 5.99M, Category = cat, Description = "Tasty", IsActive = true });
                db.SaveChanges();
            }
        }
    }
}

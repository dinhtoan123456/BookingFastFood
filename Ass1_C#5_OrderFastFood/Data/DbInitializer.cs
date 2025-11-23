using Ass1_C_5_OrderFastFood.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Ass1_C_5_OrderFastFood.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Kiểm tra xem đã có danh mục nào chưa. Nếu có, không làm gì cả.
            if (context.FoodCategories.Any())
            {
                return;
            }

            // --- 1. Tạo danh sách các danh mục mặc định ---
            var categories = new FoodCategory[]
            {
                new FoodCategory{Name = "Đồ Ăn Chính"},
                new FoodCategory{Name = "Đồ Uống / Đồ Nước"},
                new FoodCategory{Name = "Món Ăn Kèm / Đồ Khô"},
                new FoodCategory{Name = "Tráng Miệng"},
                new FoodCategory{Name = "Khuyến Mãi"}
            };

            // --- 2. Thêm vào Context và Lưu vào Database ---
            context.FoodCategories.AddRange(categories);
            context.SaveChanges();

            // Lưu ý: Nếu bạn muốn thêm các FoodItem mặc định, bạn cũng làm tương tự ở đây.
        }
        public static async Task SeedRolesAndAdminAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            // Các roles cần có
            string[] roles = { "Admin", "Customer", "Guest" };

            // Tạo roles nếu chưa có
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // ✅ Tạo tài khoản Admin mặc định
            string adminEmail = "admin@gmail.com";
            string adminPassword = "Admin@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail, // giống email
                    Email = adminEmail,
                    FullName = "Admin",
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");

                    // ✅ Thêm Claim cho Admin
                    await userManager.AddClaimAsync(adminUser, new Claim("Permission", "ManageUsers"));
                }
            }
            else
            {
                // đảm bảo admin có role + claim
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                    await userManager.AddToRoleAsync(adminUser, "Admin");

                var claims = await userManager.GetClaimsAsync(adminUser);
                if (!claims.Any(c => c.Type == "Permission" && c.Value == "ManageUsers"))
                {
                    await userManager.AddClaimAsync(adminUser, new Claim("Permission", "ManageUsers"));
                }
            }
        }
    }
}

using Ass1_C_5_OrderFastFood.Data;
using Ass1_C_5_OrderFastFood.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Ass1_C_5_OrderFastFood.Controllers
{
    public class FoodItemsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private const string ImageFolder = "images/fooditems";

        public FoodItemsController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // ✅ Danh sách món ăn
        public async Task<IActionResult> Index(string search)
        {
            var foods = _db.FoodItems.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                foods = foods.Where(f => f.Name.Contains(search));

            return View(await foods.ToListAsync());
        }


        // ✅ Tạo món ăn mới (GET)
        public async Task<IActionResult> Create()
        {
            // 1. Tải danh sách Category
            var categories = await _db.FoodCategories.ToListAsync();
            // 2. Truyền danh sách qua ViewBag
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name");

            return View();
        }

        // ✅ Tạo món ăn mới (POST) - Đã sửa lỗi và thêm TempData
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            // Thêm CategoryId vào Bind
            [Bind("Name,Description,Price,IsActive,CategoryId,Quantity")] FoodItem foodItem,
                                                                                    IFormFile ImageFile)
        {
            // Kiểm tra: File ảnh bắt buộc
            if (ImageFile == null || ImageFile.Length == 0)
            {
                ModelState.AddModelError("ImageFile", "Vui lòng chọn file ảnh cho món ăn.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // --- 1. Xử lý Upload Ảnh ---
                    string uploadPath = Path.Combine(_env.WebRootPath, ImageFolder);
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    string filePath = Path.Combine(uploadPath, fileName);

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    foodItem.ImageUrl = $"/{ImageFolder}/{fileName}";

                    // --- 2. Lưu FoodItem vào Database ---
                    _db.Add(foodItem);
                    await _db.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Đã thêm món ăn '{foodItem.Name}' thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Lỗi hệ thống: Không thể lưu món ăn. Vui lòng kiểm tra log.";
                    ModelState.AddModelError("", "Lỗi khi lưu DB: " + ex.Message);
                }
                var categories1 = await _db.FoodCategories.ToListAsync();
                ViewBag.CategoryId = new SelectList(categories1, "Id", "Name", foodItem.CategoryId);

                return View(foodItem);
            }

            // --- QUAN TRỌNG: Load lại Category nếu ModelState.IsValid = false ---
            // Nếu không có dòng này, View sẽ lỗi hoặc mất dữ liệu
            var categories = await _db.FoodCategories.ToListAsync();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", foodItem.CategoryId);

            // Trả về View với foodItem (có chứa dữ liệu đã nhập)
            return View(foodItem);
        }

        // ✅ Sửa món ăn
        public async Task<IActionResult> Edit(int id)
        {
            var food = await _db.FoodItems.FindAsync(id);
            if (food == null) return NotFound();
            return View(food);
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, FoodItem model, IFormFile file)
        {
            var food = await _db.FoodItems.FindAsync(id);
            if (food == null) return NotFound();

            if (ModelState.IsValid)
            {
                // ... (Các trường khác giữ nguyên) ...

                if (file != null && file.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    // [ĐÃ SỬA LỖI] Chuẩn hóa đường dẫn: Dùng ImageFolder thay vì "images/foods"
                    var path = Path.Combine(_env.WebRootPath, ImageFolder, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    food.ImageUrl = $"/{ImageFolder}/" + fileName;
                }

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // ✅ Xóa món ăn
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var food = await _db.FoodItems.FindAsync(id);
            if (food == null) return NotFound();

            // Optional: Xóa ảnh khỏi wwwroot
            if (!string.IsNullOrEmpty(food.ImageUrl))
            {
                var path = Path.Combine(_env.WebRootPath, food.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            _db.FoodItems.Remove(food);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

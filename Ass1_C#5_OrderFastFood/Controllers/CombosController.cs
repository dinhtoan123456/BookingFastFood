using Ass1_C_5_OrderFastFood.Data;
using Ass1_C_5_OrderFastFood.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Ass1_C_5_OrderFastFood.Controllers
{
    public class CombosController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public CombosController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // ✅ Danh sách Combo
        public async Task<IActionResult> Index()
        {
            var combos = await _db.Combos
               .Include(c => c.ComboItems)
               .ThenInclude(ci => ci.FoodItem)
               .ToListAsync();
            return View(combos);
        }

        // ✅ Tạo Combo
        public async Task<IActionResult> Create()
        {
            ViewBag.FoodItemsList = _db.FoodItems
               .Select(f => new SelectListItem
               {
                   Value = f.Id.ToString(),
                   Text = f.Name + " (" + f.Price.ToString("N0") + " VND)"
               }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComboCreateViewModel vm)
        {
            if (vm.SelectedFoodItemIds == null || vm.SelectedFoodItemIds.Length == 0)
            {
                ModelState.AddModelError("SelectedFoodItemIds", "Combo phải có ít nhất một món ăn.");
            }

            if (ModelState.IsValid)
            {
                // Lấy giá các món từ DB
                var foodItems = await _db.FoodItems
                    .Where(f => vm.SelectedFoodItemIds.Contains(f.Id))
                    .ToListAsync();

                decimal totalPriceOfItems = 0;
                for (int i = 0; i < vm.SelectedFoodItemIds.Length; i++)
                {
                    var food = foodItems.FirstOrDefault(f => f.Id == vm.SelectedFoodItemIds[i]);
                    if (food != null)
                        totalPriceOfItems += food.Price * vm.Quantities[i];
                }

                // Kiểm tra giá combo <= 95% tổng giá các món
                if (vm.Price > totalPriceOfItems * 0.95m)
                {
                    ModelState.AddModelError("Price", $"Giá Combo phải rẻ hơn ít nhất 5% so với tổng giá các món ({totalPriceOfItems:N0} VND).");
                }
            }

            if (ModelState.IsValid)
            {
                var combo = new Combo
                {
                    Name = vm.Name,
                    Price = vm.Price,
                    Description = vm.Description,
                    ComboItems = new List<ComboItem>()
                };

                // Xử lý ảnh
                if (vm.ImageFile != null)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "images/combos");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(vm.ImageFile.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await vm.ImageFile.CopyToAsync(stream);
                    }

                    combo.ImageUrl = "/images/combos/" + fileName;
                }

                // Thêm ComboItems
                for (int i = 0; i < vm.SelectedFoodItemIds.Length; i++)
                {
                    combo.ComboItems.Add(new ComboItem
                    {
                        FoodItemId = vm.SelectedFoodItemIds[i],
                        Quantity = vm.Quantities[i]
                    });
                }

                _db.Combos.Add(combo);
                await _db.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Đã tạo combo '{combo.Name}' thành công!";
                return RedirectToAction(nameof(Index));
            }

            // Reload ViewBag nếu lỗi
            ViewBag.FoodItemsList = _db.FoodItems
                .Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = f.Name + " (" + f.Price.ToString("N0") + " VND)"
                }).ToList();

            TempData["ErrorMessage"] = "Lỗi nhập liệu: Vui lòng kiểm tra lại các trường.";
            return View(vm);
        }

        // ✅ Xóa Combo
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var combo = await _db.Combos
                .Include(c => c.ComboItems)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (combo == null) return NotFound();

            // Xóa file ảnh nếu có
            if (!string.IsNullOrEmpty(combo.ImageUrl))
            {
                var filePath = Path.Combine(_env.WebRootPath, combo.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            _db.Combos.Remove(combo);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

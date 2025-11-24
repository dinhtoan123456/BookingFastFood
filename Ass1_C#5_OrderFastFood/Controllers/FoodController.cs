using Ass1_C_5_OrderFastFood.Data;
using Ass1_C_5_OrderFastFood.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Ass1_C_5_OrderFastFood.Controllers
{
    public class FoodController : Controller
    {
        private readonly ApplicationDbContext _db;
        public FoodController(ApplicationDbContext db) { _db = db; }

        public IActionResult Index(string q, decimal? minPrice, decimal? maxPrice, int? categoryId)
        {
            var items = _db.FoodItems
                            .Include(f => f.Category)
                            .Where(f => f.IsActive)
                            .AsQueryable();

            if (!string.IsNullOrEmpty(q))
                items = items.Where(f => f.Name.Contains(q));

            if (minPrice.HasValue)
                items = items.Where(f => f.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                items = items.Where(f => f.Price <= maxPrice.Value);
            if (categoryId.HasValue)
                items = items.Where(f => f.CategoryId == categoryId.Value);

            var vm = items.OrderBy(f => f.Name).ToList();
            return View(vm);
        }

        public IActionResult Details(int id)
        {
            var item = _db.FoodItems.Include(f => f.Category).FirstOrDefault(f => f.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }
        public async Task<IActionResult> Search(FoodSearchViewModel search)
        {
            // Load Categories cho dropdown
            search.Categories = await _db.FoodCategories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            var query = _db.FoodItems
                           .Include(f => f.Category)
                           .AsQueryable();

            if (!string.IsNullOrEmpty(search.Name))
                query = query.Where(f => f.Name.Contains(search.Name));

            if (search.PriceMin.HasValue)
                query = query.Where(f => f.Price >= search.PriceMin.Value);

            if (search.PriceMax.HasValue)
                query = query.Where(f => f.Price <= search.PriceMax.Value);

            // 🔥 Tìm theo CategoryId từ dropdown
            if (search.CategoryId.HasValue)
                query = query.Where(f => f.CategoryId == search.CategoryId.Value);

            // 🔍 Tìm theo mô tả
            if (!string.IsNullOrEmpty(search.Description))
                query = query.Where(f => f.Description.Contains(search.Description));

            // Lấy kết quả
            search.Results = await query.ToListAsync();

            return View(search);
        }
    }
}

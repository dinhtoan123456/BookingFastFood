using Ass1_C_5_OrderFastFood.Data;
using Microsoft.AspNetCore.Mvc;
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
    }
}

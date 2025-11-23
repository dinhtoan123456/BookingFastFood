using Ass1_C_5_OrderFastFood.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ass1_C_5_OrderFastFood.Controllers
{
    public class ComboOrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ComboOrderController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string search)
        {
            // Lấy danh sách combo kèm thành phần
            var combosQuery = _db.Combos
                .Include(c => c.ComboItems)
                .ThenInclude(ci => ci.FoodItem)
                .AsQueryable();

            // Lọc theo từ khóa tìm kiếm nếu có
            if (!string.IsNullOrEmpty(search))
            {
                combosQuery = combosQuery.Where(c => c.Name.Contains(search));
                ViewData["CurrentFilter"] = search;
            }

            var combos = await combosQuery.ToListAsync();
            return View(combos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var combo = await _db.Combos
                .Include(c => c.ComboItems)
                .ThenInclude(ci => ci.FoodItem)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (combo == null) return NotFound();

            return View(combo);
        }

    }
}

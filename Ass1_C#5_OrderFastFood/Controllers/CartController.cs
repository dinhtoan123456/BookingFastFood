using Ass1_C_5_OrderFastFood.Data;
using Ass1_C_5_OrderFastFood.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ass1_C_5_OrderFastFood.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private const string CARTKEY = "Cart";

        public CartController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var cart = GetCart(); // lấy giỏ từ session
            var user = _userManager.GetUserAsync(User).Result;

            // Địa chỉ mặc định lúc đăng ký
            ViewBag.DefaultAddress = user?.Address ?? "";

            // Lấy danh sách địa chỉ đã lưu từ bảng UserAddress
            var userId = _userManager.GetUserId(User);
            ViewBag.SavedAddresses = _db.UserAddresses
                                        .Where(a => a.ApplicationUserId == userId)
                                        .OrderByDescending(a => a.CreatedAt)
                                        .Select(a => a.Address)
                                        .ToList();

            return View(cart);
        }
        // POST: thêm địa chỉ mới
        [HttpPost]
        public async Task<IActionResult> AddAddress(string newAddress)
        {
            if (!User.Identity.IsAuthenticated || string.IsNullOrWhiteSpace(newAddress))
                return RedirectToAction("Index");

            var userId = _userManager.GetUserId(User);
            var address = new UserAddress
            {
                ApplicationUserId = userId,
                Address = newAddress
            };
            _db.UserAddresses.Add(address);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // POST: xóa địa chỉ đã lưu
        [HttpPost]
        public async Task<IActionResult> RemoveAddress(int addressId)
        {
            var address = await _db.UserAddresses.FindAsync(addressId);
            if (address != null)
            {
                _db.UserAddresses.Remove(address);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // lấy cart từ Session
        private List<CartItem> GetCart()
        {
            var sessionCart = HttpContext.Session.GetString(CARTKEY);
            if (string.IsNullOrEmpty(sessionCart))
                return new List<CartItem>();

            return JsonConvert.DeserializeObject<List<CartItem>>(sessionCart);
        }

        // lưu cart vào Session
        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString(CARTKEY, JsonConvert.SerializeObject(cart));
        }

        // Thêm món ăn vào giỏ
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var food = _db.FoodItems.FirstOrDefault(f => f.Id == id);
            if (food == null) return NotFound();

            var cart = GetCart();
            var item = cart.FirstOrDefault(ci => ci.FoodItemId == id);

            if (item != null)
                item.Quantity += quantity;
            else
                cart.Add(new CartItem
                {
                    FoodItemId = food.Id,
                    Name = food.Name,
                    UnitPrice = food.Price,
                    Quantity = quantity
                });

            SaveCart(cart);
            return RedirectToAction("Index"); // <-- đổi từ ViewCart
        }

        // Xem giỏ hàng
        public IActionResult ViewCart()
        {
            var cart = GetCart();
            return View(cart);
        }

        // Checkout
        [HttpPost]
        public async Task<IActionResult> Checkout(string deliveryAddress, string paymentMethod)
        {
            var cart = GetCart();
            if (cart.Count == 0)
                return RedirectToAction("Index");

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = _userManager.GetUserId(User);

            var order = new Order
            {
                ApplicationUserId = userId,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                DeliveryAddress = deliveryAddress,
                PaymentMethod = paymentMethod,
                Total = cart.Sum(ci => ci.UnitPrice * ci.Quantity)
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            foreach (var ci in cart)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    FoodItemId = ci.FoodItemId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice
                };
                _db.OrderItems.Add(orderItem);
            }

            await _db.SaveChangesAsync();
            HttpContext.Session.Remove(CARTKEY);

            // Thêm thông báo thành công
            TempData["SuccessMessage"] = "Thanh toán thành công! Đơn hàng của bạn đã được ghi nhận.";

            return RedirectToAction("Index", "Cart"); // Redirect về trang giỏ hàng để hiển thị thông báo
        }
        // Thêm combo vào giỏ
        [HttpPost]
        public IActionResult AddCombo(int id, int quantity = 1)
        {
            var combo = _db.Combos.Include(c => c.ComboItems)
                                   .ThenInclude(ci => ci.FoodItem)
                                   .FirstOrDefault(c => c.Id == id);
            if (combo == null) return NotFound();

            var cart = GetCart();
            var item = cart.FirstOrDefault(ci => ci.ComboId == id);

            if (item != null)
                item.Quantity += quantity;
            else
                cart.Add(new CartItem
                {
                    ComboId = combo.Id,
                    Name = combo.Name,
                    UnitPrice = combo.Price,
                    Quantity = quantity
                });

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        [HttpPost] // POST để bảo mật xóa
        public IActionResult Remove(int? foodItemId = null, int? comboId = null)
        {
            var cart = GetCart();

            CartItem item = null;
            if (foodItemId.HasValue)
                item = cart.FirstOrDefault(ci => ci.FoodItemId == foodItemId.Value);
            else if (comboId.HasValue)
                item = cart.FirstOrDefault(ci => ci.ComboId == comboId.Value);

            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> History(string? range)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var query = _db.Orders
                .Where(o => o.ApplicationUserId == user.Id)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.FoodItem)
                .AsQueryable();

            DateTime now = DateTime.Now;

            switch (range)
            {
                case "today":
                    query = query.Where(o => o.CreatedAt.Date == now.Date);
                    break;

                case "week":
                    query = query.Where(o => o.CreatedAt >= now.AddDays(-7));
                    break;

                case "month":
                    query = query.Where(o => o.CreatedAt >= now.AddMonths(-1));
                    break;

                case "year":
                    query = query.Where(o => o.CreatedAt >= now.AddYears(-1));
                    break;
            }

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }
        public async Task<IActionResult> OrderDetail(int id)
        {
            var order = await _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.FoodItem)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }


    }
}

using Ass1_C_5_OrderFastFood.Data;
using Ass1_C_5_OrderFastFood.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ass1_C_5_OrderFastFood.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Ass1_C_5_OrderFastFood.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender; // service gửi mail

        public OrdersController(ApplicationDbContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }

        // ✅ Danh sách đơn hàng (lọc theo trạng thái)
        public IActionResult Index(OrderStatus? status)
        {
            var orders = _db.Orders
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.FoodItem)
                .AsQueryable();

            if (status != null)
                orders = orders.Where(o => o.Status == status);

            ViewBag.CurrentStatus = status;
            return View(orders.OrderByDescending(o => o.CreatedAt).ToList());
        }

        // ✅ Xem chi tiết đơn hàng
        public IActionResult Details(int id)
        {
            var order = _db.Orders
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.FoodItem)
                .FirstOrDefault(o => o.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        // ✅ Cập nhật trạng thái
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, OrderStatus status)
        {
            var order = _db.Orders.Include(o => o.ApplicationUser).FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();

            order.Status = status;
            _db.SaveChanges();

            // nếu đơn bị hủy → gửi email cho khách
            if (status == OrderStatus.Cancelled && order.ApplicationUser != null)
            {
                var email = order.ApplicationUser.Email;
                var subject = $"Đơn hàng #{order.Id} đã bị hủy";
                var message = $"Xin chào {order.ApplicationUser.UserName},\n\nĐơn hàng #{order.Id} đã bị hủy bởi quản trị viên.\nNếu có thắc mắc, vui lòng liên hệ CSKH.";
                await _emailSender.SendEmailAsync(email, subject, message);
            }

            return RedirectToAction("Details", new { id });
        }
    }
}

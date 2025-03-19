using Microsoft.AspNetCore.Mvc;
using NguyenQuocVuong_Nhom11.Models;
using NguyenQuocVuong_Nhom11.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace NguyenQuocVuong_Nhom11.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)] // Chỉ Admin mới truy cập được
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // 1️⃣ Hiển thị danh sách đơn hàng
        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepository.GetAllAsync();
            return View(orders);
        }

        // 2️⃣ Hiển thị chi tiết đơn hàng
        public async Task<IActionResult> Display(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // 3️⃣ Xóa đơn hàng
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _orderRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

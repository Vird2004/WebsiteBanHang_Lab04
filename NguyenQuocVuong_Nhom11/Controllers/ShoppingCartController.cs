using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NguyenQuocVuong_Nhom11.DataAccess;
using NguyenQuocVuong_Nhom11.Extensions;
using NguyenQuocVuong_Nhom11.Models;
using NguyenQuocVuong_Nhom11.Repositories;

namespace NguyenQuocVuong_Nhom11.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartService _cartService; // 🔹 Khai báo _cartService
        public ShoppingCartController(ApplicationDbContext context,
    UserManager<ApplicationUser> userManager, IProductRepository
    productRepository)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = SD.Role_Customer)]
        public IActionResult Checkout()
        {
            return View(new Order());
        }

        [Authorize(Roles = SD.Role_Customer)]
        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart =
            HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any())
            {
                // Xử lý giỏ hàng trống... 
                return RedirectToAction("Index");
            }
            var user = await _userManager.GetUserAsync(User);
            order.UserId = user.Id;
            order.OrderDate = DateTime.UtcNow;
            order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
            order.OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList();
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("Cart");
            return View("OrderCompleted", order.Id); // Trang xác nhận hoàn 
            //thành đơn hàng
}

        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return NotFound();

            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            var cartItem = cart.Items.FirstOrDefault(p => p.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    ImageUrl = product.ImageUrl // ✅ Lưu đường dẫn ảnh vào giỏ hàng
                });
            }

            UpdateCartSession(cart);

            // 🔄 Điều hướng quay lại trang Index cua HomeController
            return RedirectToAction("Index", "Home");

        }


        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            return View(cart);
        }

        // Các actions khác... 
        private async Task<Product> GetProductFromDatabase(int productId)
        {
            // Truy vấn cơ sở dữ liệu để lấy thông tin sản phẩm 
            var product = await _productRepository.GetByIdAsync(productId);
            return product;
        }
        private void UpdateCartSession(ShoppingCart cart)
        {
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            HttpContext.Session.SetInt32("CartItemCount", cart.Items.Sum(i => i.Quantity));
        }



        [HttpPost] // Chỉ chấp nhận yêu cầu POST để tránh lỗi 405
        public IActionResult RemoveFromCart([FromBody] int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart != null)
            {
                cart.RemoveItem(productId);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return Json(new { success = true, message = "Sản phẩm đã được xóa." });
        }


        [HttpGet]
        public IActionResult GetCartItemCount()
        {
            int count = HttpContext.Session.GetInt32("CartItemCount") ?? 0;
            return Json(new { count });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            var cartItem = cart.Items.FirstOrDefault(p => p.ProductId == productId);

            if (cartItem != null && quantity > 0)
            {
                cartItem.Quantity = quantity;
                HttpContext.Session.SetObjectAsJson("Cart", cart);
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            return PartialView("_CartPartial", new List<CartItem>());
        }


        [HttpPost]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(p => p.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return PartialView("_CartPartial", cart);
        }

        [HttpGet]
        public IActionResult GetCartCount()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            int count = cart.Sum(i => i.Quantity);
            return Content(count.ToString());
        }

    }
}

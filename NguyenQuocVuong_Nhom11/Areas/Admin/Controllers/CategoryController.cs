using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NguyenQuocVuong_Nhom11.Models;
using NguyenQuocVuong_Nhom11.Repositories;
using System.Threading.Tasks;

namespace NguyenQuocVuong_Nhom11.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)] // Chỉ Admin mới có quyền
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // Hiển thị danh sách danh mục sản phẩm
        [AllowAnonymous] // Cho phép cả khách xem danh mục
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }
    }
}

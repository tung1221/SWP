using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System.Linq;

namespace Project.Controllers
{
    public class BlogController : Controller
    {
        private readonly ShopContext _context;

        public BlogController(ShopContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var result = from a in _context.Blogs
                         join b in _context.ImageBlogs on a.Blogid equals b.BlogId
                         select new { Blog = a, ImageBlog = b };

            var blogs = result.Select(r => r.Blog).ToList();
            var imageBlogs = result.Select(r => r.ImageBlog).ToList();

            var viewModel = new BlogViewModel
            {
                Blogs = blogs,
                ImageBlogs = imageBlogs
            };

            return View(viewModel);
        }

        public IActionResult BlogDetails(int blogId)
        {
            // Lấy thông tin blog từ cơ sở dữ liệu
            var blog = _context.Blogs.FirstOrDefault(b => b.Blogid == blogId);

            if (blog == null)
            {
                // Xử lý khi không tìm thấy blog
                return NotFound();
            }

            // Lấy danh sách ảnh tương ứng với blogId
            var images = _context.ImageBlogs.Where(i => i.BlogId == blogId).ToList();

            // Tạo ViewModel và truyền thông tin blog và danh sách ảnh vào
            var viewModel = new BlogViewModel
            {
                Blogs = new List<Blog> { blog },
                ImageBlogs = images
            };

            // Trả về View hiển thị thông tin blog và danh sách ảnh
            return View(viewModel);
        }





    }
}

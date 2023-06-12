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

            ViewData["Blogs"] = blogs;
            ViewData["ImageBlogs"] = imageBlogs;

            return View();
        }

        public IActionResult BlogDetails(int blogId)
        {
            var viewProductById = _context.Products
                .Where(p => p.HomeStatus == true && p.BlogId == blogId)
                .ToList();

            var blog = _context.Blogs.FirstOrDefault(b => b.Blogid == blogId);

            if (blog == null)
            {
                return NotFound();
            }

            var images = _context.ImageBlogs.Where(i => i.BlogId == blogId).ToList();

            ViewData["Blog"] = blog;
            ViewData["ImageBlogs"] = images;
            ViewData["ViewProductById"] = viewProductById;

            return View();
        }







    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Project.Data;
using Project.Models;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection.Metadata;

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
            var blogs = _context.Blogs
            .Include(b => b.ImageBlogs)
            .Where(b => b.ImageBlogs.Any(ib => ib.IsBigImg == true))
            .Select(b => new
            {
                b.name,
                b.Blogid,
                b.HomeStatus,
                b.content,
                b.DateUp,
                FirstImage = b.ImageBlogs.Single(ib => ib.IsBigImg == true).ImageURL
            })
            .ToList();



            return View(blogs);
        }


        public IActionResult BlogDetails(int blogId)
        {
            var check = _context.Blogs.Find(blogId);

            if (check == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var blog = _context.Blogs
               .Include(b => b.Products)
               .Include(b => b.ImageBlogs)
               .FirstOrDefault(b => b.Blogid == blogId);
                return View(blog);
            }
        }
    }
}

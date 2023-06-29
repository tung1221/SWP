using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Project.Data;
using Project.Models;
using System.Drawing.Imaging;
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
			var blogs = _context.Blogs
			.Include(b => b.ImageBlogs)
			.Where(b => b.HomeStatus == true && b.ImageBlogs.Any(ib => ib.IsBigImg == true))
			.Select(b => new
			{
				b.Blogid,
				b.HomeStatus,
				b.content,
				b.DateUp,
				FirstImage = b.ImageBlogs.Single(ib => ib.IsBigImg == true).ImageURL
			})
			.ToList();


			return View(blogs);
		}


		public async Task<IActionResult> BlogDetails(int blogId)
		{
			var blog = await _context.Blogs
				.Include(b => b.Products)
				.Include(b => b.ImageBlogs)
				.FirstOrDefaultAsync(b => b.Blogid == blogId);


			if (blog == null)
			{
				return RedirectToAction("Index");
			}


			ViewData["Blog"] = blog;
			return View();
		}








	}









}

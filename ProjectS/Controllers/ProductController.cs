using Microsoft.AspNetCore.Mvc;
using Project.Data;
using Project.Models;
using System.Collections.Generic;

namespace Project.Controllers
{
    public class ProductController : Controller
    {

        private readonly ILogger<ProductController> _logger;
        private readonly ShopContext _shopContext;
        public ProductController(ILogger<ProductController> logger, ShopContext ct)
        {
            _shopContext = ct;
            _logger = logger;
        }


        public IActionResult Index(int id, bool gender)
        {
            List<SubCategory> subCategories = new List<SubCategory>();
            var list = (from p in _shopContext.Products
                        where p.SubCategoryID == id
                        select p).ToList();
            var subCate = (from c in _shopContext.SubCategory
                           where c.SubCategoryId == id
                           select c).FirstOrDefault();
            var cateGory = (from c in _shopContext.Categories
                            where c.CategoryId == subCate.CateogoryId
                            select c).First();
            if (cateGory != null)
            {
                var e = _shopContext.Entry(cateGory);
                e.Collection(c => c.SubCategories).Load();
                subCategories = cateGory.SubCategories;
            }
            subCategories.Sort((x, y) => x.SubCategoryId.CompareTo(y.SubCategoryId));

            ViewData["listSubCate"] = subCategories;
            ViewData["subCate"] = subCate;
            ViewData["gender"] = gender;

            return View(list);

        }

        public IActionResult DetailProduct(int id)
        {
            var product = _shopContext.Products.Where(p => p.ProductId == id).FirstOrDefault();



            if (product != null)
            {
                ViewData["productId"] = product.ProductId;
                ViewData["image"] = product.ImageMain;
                var e = _shopContext.Entry(product);
                e.Collection(c => c.ImageProducts).Load();
            }
            else
            {
                return RedirectToAction("Index", "Home", new { mode = "EDetailProduct" });
            }

            return View(product);
        }
    }
}

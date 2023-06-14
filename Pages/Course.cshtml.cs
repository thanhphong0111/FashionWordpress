using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Group1_CourseOnline.Pages
{
    public class CourseModel : PageModel
    {
        public readonly SWP391_BlueEduContext _db;
        public CourseModel(SWP391_BlueEduContext db)
        {
            _db = db;
        }
        [BindProperty(SupportsGet = true)]
        public string CategoryName { get; set; }
        [BindProperty]
        public List<Product> Products { get; set; }
        [BindProperty]
        public List<Category> Categories { get; set; }
        private int pageSize = 6;
        public decimal totalPage;
        public int catid;
        public int pageNumber;
        public async Task<IActionResult> OnGet(int category, string categoryName, int pagenumber)
        {
            catid = category;
            Categories = _db.Categories.ToList();

            if (string.IsNullOrEmpty(categoryName))
            {
                CategoryName = null;
                ViewData["Title"] = "List Course";
            }
            else
            {
                CategoryName = categoryName;
                ViewData["Title"] = CategoryName;
            }

            if (_db.Products != null)
            {
                var queryProduct = (from item in _db.Products
                                join c in _db.Categories
                                on item.CategoryId equals c.CategoryId
                                where ((category==0)?true:item.CategoryId==category) 
                                select new
                                {
                                    Cmt = (from cmt in _db.Comments where item.ProductId == cmt.ProductId select cmt).Count(),
                                    item.ProductId,
                                    item.UnitPrice,
                                    item.ProductName,
                                    item.Employee,
                                    item.Image,
                                    item.Views,
                                    c.CategoryId
                                }).ToList();
                if(queryProduct.Count() % pageSize != 0)
                {
                    totalPage = queryProduct.Count() / pageSize+1;

                }
                else
                {
                    totalPage = queryProduct.Count() / pageSize;
                }
                if (pagenumber <= 0)
                {
                    pagenumber = 1;
                }
                pageNumber = pagenumber;

                var products = queryProduct.Skip((pagenumber-1)*pageSize).Take(pageSize).ToList();
                ViewData["products"] = products;
            }
            return Page();
        }

    }
}

using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Group1_CourseOnline.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SWP391_BlueEduContext _db;
        public IndexModel(SWP391_BlueEduContext db)
        {
            _db = db;
        }
        [BindProperty]
        public List<Category> Categories { get; set; }
        [BindProperty]
        public List<News> News { get; set; }
        public List<News> NewsInBanner { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var Products = (from item in _db.Products
                            join c in _db.Categories
                            on item.CategoryId equals c.CategoryId
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
            ViewData["product"] = Products.Where(e => e.UnitPrice != 0);
            ViewData["FreeProduct"] = Products.Where(e => e.UnitPrice == 0); ;

            var Comment = (from item in _db.Comments
                           join c in _db.Customers
                           on item.CustomerId equals c.CustomerId
                           select new
                           {
                               item.CommentTime,
                               item.CommentId,
                               c.CustomerId,
                               item.Content,
                               item.Rating,
                               Image = c.Avatar,
                               FirstName = c.FirstName,
                               LastName = c.LastName,
                           }).Take(8).ToList();
            ViewData["Comments"] = Comment;
            Categories = _db.Categories.ToList();
            News = _db.News.OrderByDescending(e =>e.CreateDate).ToList();
            NewsInBanner = _db.News.Where(e =>e.CategoryNewId ==3).OrderByDescending(e => e.CreateDate).ToList();
            return Page();

        }
    }
}

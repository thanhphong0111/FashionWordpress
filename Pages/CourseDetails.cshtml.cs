using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Group1_CourseOnline.Pages
{
    public class CourseDetailsModel : PageModel
    {
        public readonly SWP391_BlueEduContext _db;
        public CourseDetailsModel(SWP391_BlueEduContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Product Products { get; set; }
        [BindProperty]
        public List<Lesson> Lessons { get; set; }
        [BindProperty]

        
        public List<LessonVideo> LessonVideos { get; set; }

        public bool CheckOwn { get; set; }
        public async Task<IActionResult> OnGet(int productid)
        {
            var customer = HttpContext.Session.GetString("customer");
            Products = _db.Products.Include(e => e.Employee).Where(e => e.ProductId == productid).FirstOrDefault();
            if (customer != null)
            {
                var customerData = JsonSerializer.Deserialize<Customer>(customer);
                var OrderOfCus = _db.OrderDetails.Include(o => o.Order).Where(c => c.ProductId == productid).Where(c => c.Order.CustomerId == customerData.CustomerId).ToList();
                if (OrderOfCus.Count == 0)
                {
                    CheckOwn =false;
                    Products.Views = Products.Views + 1;
                    _db.Products.Update(Products);
                    _db.SaveChanges();
                }
                else
                {
                    CheckOwn =true;
                }
            }
            else
            {
                Products.Views = Products.Views + 1;
                _db.Products.Update(Products);
                _db.SaveChanges();
            }

           
            var Comment = (from item in _db.Comments
                           join c in _db.Customers
                           on item.CustomerId equals c.CustomerId
                           where item.ProductId == productid
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
                           }).Take(4).ToList();
            ViewData["Comments"] = Comment;

            var ListProduct = (from item in _db.Products
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
            ViewData["product"] = ListProduct.Where(e => e.CategoryId == Products.CategoryId);
            Lessons = _db.Lessons.Where(p => p.ProductId == productid).ToList();
            LessonVideos = (from item in _db.LessonVideos
                            join c in _db.Lessons
                            on item.LessonId equals c.LessonId
                            where c.ProductId == productid
                            select item
                            ).ToList();

            return Page();
        }


    }
}

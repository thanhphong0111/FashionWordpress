using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

namespace Group1_CourseOnline.Pages.Admin.Customers
{
    public class DetailsModel : PageModel
    {
        public readonly SWP391_BlueEduContext _db;
        public DetailsModel(SWP391_BlueEduContext db)
        {
            _db = db;
        }
        [BindProperty]
        public Customer Customers { get; set; }

        public int CommentsNumber { get; set; }
        public int CourseNumber { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            if (HttpContext.Session.GetString("admin") == null)
            {
                return RedirectToPage("/Login");
            }
            if (id == 0)
            {
                return NotFound();
            }
            else
            {

                Customers = _db.Customers.Find(id);
                CommentsNumber = _db.Comments.Where(c => c.CustomerId == id).Count();
                CourseNumber = _db.Orders.Where(c => c.CustomerId == id).Count();
                var Comment = (from item in _db.Comments
                               join course in _db.Products
                               on item.ProductId equals course.ProductId
                               join c in _db.Customers
                               on item.CustomerId equals c.CustomerId
                               where item.CustomerId == id
                               select new
                               {
                                   ProductName = course.ProductName,
                                   ProductImage = course.Image,
                                   CommentTime = (DateTime.Now - item.CommentTime).Days,
                                   item.CommentId,
                                   c.CustomerId,
                                   item.Content,
                                   item.Rating,
                                   Image = c.Avatar,
                                   FirstName = c.FirstName,
                                   LastName = c.LastName,
                               }).ToList();
                ViewData["Comments"] = Comment;

                var owmCourse = (from o in _db.Orders
                                join od in _db.OrderDetails on o.OrderId equals od.OrderId
                                join p in _db.Products on od.ProductId equals p.ProductId
                                where o.CustomerId == id
                                 orderby o.OrderDate descending
                                 select new
                                {
                                    p.ProductName,
                                    p.Image,
                                    FirstName= p.Employee.FirstName,
                                    LastName= p.Employee.LastName,
                                    OrderDate = o.OrderDate.Value.ToString("dd/MM/yyyy"),
                                    OrderTime = o.OrderDate.Value.Hour+":"+ o.OrderDate.Value.Minute
                                }).ToList();
                ViewData["owmCourse"] = owmCourse;
            }
            return Page();
        }
    }
}

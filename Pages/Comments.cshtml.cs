using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Group1_CourseOnline.Pages
{
    public class CommentsModel : PageModel
    {
        public readonly SWP391_BlueEduContext _db;
        public CommentsModel(SWP391_BlueEduContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
        }
        [HttpPost]
        public async Task<IActionResult> OnPost(string feedback, int id)
        {
            var customer = HttpContext.Session.GetString("customer");
            var customerData = JsonSerializer.Deserialize<Customer>(customer);
            try
            {
                if (!string.IsNullOrEmpty(feedback))
                {
                    Comment c = new Comment
                    {
                        Content = feedback,
                        ProductId = id,
                        CustomerId = customerData.CustomerId,
                        CommentTime = DateTime.Now
                    };

                    _db.Comments.Add(c);
                    await _db.SaveChangesAsync();
                }

            }
            catch (DbUpdateException ex) { 
                   Console.WriteLine(ex.Message);
            }


            return RedirectToPage("CourseDetails", new { productid = id });
        }

    }
}
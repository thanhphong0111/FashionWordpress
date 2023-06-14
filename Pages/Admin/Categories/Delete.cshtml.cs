using Group1_CourseOnline.Helpers;
using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Group1_CourseOnline.Pages.Admin.Categories
{
    public class DeleteModel : PageModel
    {
        public readonly SWP391_BlueEduContext _db;
        public DeleteModel(SWP391_BlueEduContext db)
        {
            _db = db;
        }
        public void OnGet()
        {
        }

        public IActionResult OnPost(int categoryId)
        {
            // Retrieve the category from the database
            var category = _db.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null)
            {
                return NotFound();
            }

            try
            {
                // Remove the category from the database
                _db.Categories.Remove(category);
                _db.SaveChanges();

                MessageBox("Success", "Category deleted successfully.", "alert-success");
            }
            catch (Exception ex)
            {
                // Handle any errors that occurred during deletion
                MessageBox("Error", "Delete the failed course category.", "alert-danger");
            }

            return RedirectToPage("/Admin/Categories/Index");
        }

   

        public void MessageBox(string Notification, string AlertMessage, string AlertType)
        {
            TempData["Notification"] = Notification;
            TempData["AlertMessage"] = AlertMessage;
            TempData["AlertType"] = AlertType;
        }
    }
}

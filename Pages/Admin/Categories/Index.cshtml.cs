using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Group1_CourseOnline.Pages.Admin.Categories
{
    public class IndexModel : PageModel
    {
        public readonly SWP391_BlueEduContext _db;
        public IndexModel(SWP391_BlueEduContext db)
        {
            _db = db;
        }

        [BindProperty]
        public List<Category> Categories { get; set; }

        [BindProperty]
        public Category Category { get; set; }
        public int id { get; set; }
        public IActionResult OnGet(int categoryId)
        {
            id = categoryId;
            Category = _db.Categories.Where(e => e.CategoryId == id).FirstOrDefault();
            if (HttpContext.Session.GetString("admin") == null)
            {
                return RedirectToPage("/Login");
            }
            Categories = _db.Categories.ToList();
            return Page();
        }

        public IActionResult OnPost(int categoryId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    if (categoryId == 0)
                    {
                        Category category = new Category
                        {
                            CategoryName = Category.CategoryName,
                            Description = Category.Description,
                            CreateBy = "Admin",
                            CreateTime = DateTime.Now,
                        };
                        _db.Categories.Add(category);
                        _db.SaveChanges();
                        MessageBox("Success", "Create a successful course catalog.", "alert-success");
                    }
                    else
                    {
                        var category = _db.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
                        if (category != null)
                        {
                            category.CategoryName = Category.CategoryName;
                            category.Description = Category.Description;
                            category.ModifireBy = "Admin";
                            category.ModifireTime = DateTime.Now;
                            _db.SaveChanges();
                            MessageBox("Success", "Update the course catalog successfully.", "alert-success");
                        }
                    }

                    return RedirectToPage("/Admin/Categories/Index");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occurred during the process
                MessageBox("Error", "An error occurred while processing the request.", "alert-danger");
            }

            return Page();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDeleteCategoriesSelectItem(List<int> selectedCategoryId)
        {
            var categoriesToDelete = await _db.Categories
                .Where(c => selectedCategoryId.Contains(c.CategoryId))
                .ToListAsync();

            if (categoriesToDelete.Count == 0)
            {
                MessageBox("Warning", "Please select the course category you want to delete.", "alert-warning");
                return RedirectToPage("/Admin/Categories/Index");
            }

            try
            {
                foreach (var category in categoriesToDelete)
                {
                    _db.Categories.Remove(category);
                }

                await _db.SaveChangesAsync();

                MessageBox("Success", "Category deleted successfully.", "alert-success");
            }
            catch (Exception ex)
            {
                MessageBox("Error", "An error occurred while deleting the categories.", "alert-danger");
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

using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Group1_CourseOnline.Pages.Admin.Menus
{
    public class IndexModel : PageModel
    {
        public readonly SWP391_BlueEduContext _db;
        public IndexModel(SWP391_BlueEduContext db)
        {
            _db = db;
        }
        [BindProperty]
        public List<Menu> Menus { get; set; }
        [BindProperty]
        public Menu Menu { get; set; }
        public int id { get; set; }
        public IActionResult OnGet(int menuId, bool status)
        {
            id = menuId;
            if (HttpContext.Session.GetString("admin") == null)
            {
                return RedirectToPage("/Login");
            }
            else
            {
                Menu = _db.Menus.Where(e => e.MenuId == id).FirstOrDefault();
                Menus = _db.Menus.ToList();
            }
            return Page();
        }

        public IActionResult OnPost(int menuId)
        {
            try
            {
                if (menuId == 0)
                {
                    Menu menu = new Menu
                    {
                        MenuName = Menu.MenuName,
                        Url = Menu.Url,
                        Status = true
                    };
                    _db.Menus.Add(menu);
                    _db.SaveChanges();
                    MessageBox("Success", "Create a menu successful.", "alert-success");
                }
                else
                {
                    var menu = _db.Menus.FirstOrDefault(c => c.MenuId == menuId);
                    if (menu != null)
                    {
                        menu.MenuName = Menu.MenuName;
                        menu.Url = Menu.Url;
                        _db.SaveChanges();
                        MessageBox("Success", "Update the menu successfully.", "alert-success");
                    }
                }

                return RedirectToPage("/Admin/Menus/Index");
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
        public async Task<IActionResult> OnPostDisabelViewSelectItem(List<int> selectedCategoryId)
        {
            var menuToDisable = await _db.Menus
                .Where(c => selectedCategoryId.Contains(c.MenuId))
                .ToListAsync();

            if (menuToDisable.Count == 0)
            {
                MessageBox("Warning", "Please select the meny you want to disable.", "alert-warning");
                return RedirectToPage("/Admin/Menus/Index");
            }

            try
            {
                foreach (var menu in menuToDisable)
                {
                    menu.Status = !menu.Status;
                    _db.Menus.Update(menu);
                }

                await _db.SaveChangesAsync();

                MessageBox("Success", "Change status successfully.", "alert-success");
            }
            catch (Exception ex)
            {
                MessageBox("Error", "An error occurred while change status.", "alert-danger");
            }
            return RedirectToPage("/Admin/Menus/Index");
        }

        public void MessageBox(string Notification, string AlertMessage, string AlertType)
        {
            TempData["Notification"] = Notification;
            TempData["AlertMessage"] = AlertMessage;
            TempData["AlertType"] = AlertType;
        }
    }

}

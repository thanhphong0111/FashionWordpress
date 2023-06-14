using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Group1_CourseOnline.Pages.Admin.Menus
{
    public class DisableViewModel : PageModel
    {
        public readonly SWP391_BlueEduContext _db;
        public DisableViewModel(SWP391_BlueEduContext db)
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
            try
            {
                if (menuId != 0)
                {
                    var menu = _db.Menus.FirstOrDefault(c => c.MenuId == menuId);
                    if (menu != null)
                    {
                        if (status == true)
                        {
                            menu.Status = status;
                            MessageBox("Success", "Enabel view menu successful.", "alert-success");
                        }
                        else
                        {
                            menu.Status = status;
                            MessageBox("Success", "Disabel view menu successful.", "alert-success");
                        }
                        _db.Menus.UpdateRange(menu);
                        _db.SaveChanges();
                    }
                }

                return RedirectToPage("index");
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occurred during the process
                MessageBox("Error", "An error occurred while processing the request.", "alert-danger");
            }

            return RedirectToPage("index");
        }
        public void MessageBox(string Notification, string AlertMessage, string AlertType)
        {
            TempData["Notification"] = Notification;
            TempData["AlertMessage"] = AlertMessage;
            TempData["AlertType"] = AlertType;
        }
    }
}

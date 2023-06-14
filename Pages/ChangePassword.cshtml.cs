using Group1_CourseOnline.Helpers;
using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Group1_CourseOnline.Pages
{
    public class ChangePasswordModel : PageModel
    {
        private readonly SWP391_BlueEduContext _db;
        public ChangePasswordModel(SWP391_BlueEduContext db)
        {
            _db = db;
        }

        [BindProperty]
        public string OldPassword { get; set; }
        [BindProperty]
        public string NewPassword { get; set; }
        [BindProperty]
        public string RePassword { get; set; }
        [BindProperty]
        public Account Accounts { get; set; }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            
            var jsonString = HttpContext.Session.GetString("customer");
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            var myObject = JsonSerializer.Deserialize<Account>(jsonString, options);
            Accounts = myObject as Account;
            var acc = await _db.Accounts.SingleOrDefaultAsync(a => a.Email.Equals(Accounts.Email));
            
            try
            {
                string patternPass = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";
                string oldPassword = Password_encryption.HashPassWord(OldPassword).ToLower();
				if (Password_encryption.HashPassWord(NewPassword).ToLower() == acc.Password.ToLower())
				{
					ViewData["msg-fail"] = "New password already exsit!";
					return Page();
				}
                if (!Regex.IsMatch(NewPassword, patternPass))
                {
                    ViewData["msg-fail"] = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and have a minimum length of 8 characters.";
                    return Page();
                }
                if (RePassword != NewPassword)
				{
					ViewData["msg-repassword"] = "Re-password not match!";
					return Page();
				}
				if (acc.Password.ToLower().Equals(oldPassword))
                {
                    acc.Password = Password_encryption.HashPassWord(NewPassword);
                    _db.Accounts.Update(acc);
                    await _db.SaveChangesAsync();
                    ViewData["msg-success"] = "Change password successful";
                }
                else
                {
                    ViewData["msg-fail"] = "Old password is incorrect!";
                    return Page();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return Page();
            }

            return Page();
        }


    }
}
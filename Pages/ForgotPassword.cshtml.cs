using Group1_CourseOnline.Helpers;
using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Group1_CourseOnline.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly SWP391_BlueEduContext _db;
        public ForgotPasswordModel(SWP391_BlueEduContext db)
        {
            _db = db;
        }
        [BindProperty]
        public string email { get; set; }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewData["msg"] = "Please enter your email to get password!";
                return Page();
            }
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (!Regex.IsMatch(email, pattern))
            {
                ViewData["msg"] = "Please enter the correct email format ex: abc@gmail.com";
                return Page();
            }
            var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == email);
            if (account == null)
            {
                ViewData["msg"] = "Email not found in the system, please check again!";
                return Page();
            }
                string passwordGenerate = Password_encryption.GeneratePassword(8);
                account.Password = Password_encryption.HashPassWord(passwordGenerate);
                _db.Update(account);
            if (await _db.SaveChangesAsync() <= 0)
            {
                ViewData["msg"] = "System error, please try again!";
                return Page();
            }

            var body = $@"<p>Change your password: </p><br/><a href='https://localhost:7093/ResetPassword?email={email}'>Click here</a>.. new password: " + passwordGenerate;
            SendMailHelper.SendMail(email, body);
            ViewData["msgsuc"] = "Please check your email!";
            return Page();
        }
    }
}

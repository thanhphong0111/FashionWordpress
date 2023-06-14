using Group1_CourseOnline.Helpers;
using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;

namespace Group1_CourseOnline.Pages
{
    public class ResetPasswordModel : PageModel
    {
        public readonly SWP391_BlueEduContext _db;
        public ResetPasswordModel(SWP391_BlueEduContext db)
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
        public string Email { get; set; }
        public IActionResult OnGet(string email)
        {
            Email = email;
            if(Email == null)
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            if (RePassword != NewPassword)
            {
                ViewData["msg-repassword"] = "Re-password not match!";
                return Page();
            }
           
            var acc = await _db.Accounts.SingleOrDefaultAsync(a => a.Email.Equals(Email));

            try
            {
                string patternPass = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";
                if (!Regex.IsMatch(NewPassword, patternPass))
                {
                    ViewData["msg-newpassword"] = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and have a minimum length of 8 characters.";
                    return Page();
                }
                string oldPassword = Password_encryption.HashPassWord(OldPassword);
                if (acc.Password.Equals(oldPassword)) {
                acc.IsSocialAccount = false;
                acc.Password = Password_encryption.HashPassWord(NewPassword);
                _db.Accounts.Update(acc);
                await _db.SaveChangesAsync();
                return RedirectToPage("/Login");
                }
                else
                {
                    ViewData["msg-repassword"] = "Old password is incorrect!";
                    return Page();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return Page();
            }
        }
    }
}

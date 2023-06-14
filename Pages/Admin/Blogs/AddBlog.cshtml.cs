using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Text.RegularExpressions;

namespace Group1_CourseOnline.Pages.Admin.Blogs
{
    public class AddBlogModel : PageModel
    {
		public readonly SWP391_BlueEduContext _db;
		public AddBlogModel(SWP391_BlueEduContext db)
		{
			_db = db;
		}
		[BindProperty]
		public List<NewCategory> NewCategory  { get; set; }

		public News News { get; set; }
		public IActionResult OnGet()
        {
			NewCategory = _db.NewCategories.ToList();
			return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                NewCategory = _db.NewCategories.ToList();
                return Page();
            }

            // Lấy giá trị từ form
            int categoryId = Convert.ToInt32(Request.Form["Category"]);
            string title = Request.Form["Title"];
            string content = Request.Form["NewsContent"];

            // Check if the 'NewsContent' field is empty or contains only images
            string trimmedContent = content?.Trim();
            if (string.IsNullOrEmpty(trimmedContent) || IsImageOnlyContent(trimmedContent))
            {
                // Handle the case where 'NewsContent' is empty or contains only images
                // For example, display an error message or provide a default value
                ModelState.AddModelError(string.Empty, "The NewsContent field is required.");
                NewCategory = _db.NewCategories.ToList();
                return Page();
            }

            // Tạo đối tượng News và gán giá trị
            var news = new News
            {
                NewsTitle = title,
                NewsContent = WebUtility.HtmlEncode(content),
                CategoryNewId = categoryId,
                CreateDate = DateTime.Now
            };

            _db.News.Add(news);
            _db.SaveChanges();

            return RedirectToPage("/Admin/Blogs/Index");
        }

        private bool IsImageOnlyContent(string content)
        {
            // Remove HTML tags to check if the content contains only images
            string textOnlyContent = Regex.Replace(content, "<[^>]+>", string.Empty);
            textOnlyContent = WebUtility.HtmlDecode(textOnlyContent);

            // Check if the remaining content consists only of whitespace or is empty
            return string.IsNullOrWhiteSpace(textOnlyContent);
        }
    }
}

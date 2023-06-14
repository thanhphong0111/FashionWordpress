using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Group1_CourseOnline.Pages
{
    public class BlogDetailModel : PageModel
    {
        public readonly SWP391_BlueEduContext _db;
        public BlogDetailModel(SWP391_BlueEduContext db)
        {
            _db = db;
        }

        [BindProperty]
        public List<News> LatestNews { get; set; }
        public News News { get; set; }
        public async Task<IActionResult> OnGet(int id) // Thêm tham số id để truyền thông tin về tin tức được chọn
        {
            News = await _db.News.FindAsync(id); // Tìm kiếm tin tức theo id

            
                
            

            if (News == null)
            {
                return Page(); // Trả về 404 Not Found nếu không tìm thấy tin tức
            }
            News.Views = News.Views + 1;
                _db.News.Update(News);

                _db.SaveChanges();
            LatestNews = await GetTop5LatestNews();

            return Page();
        }

        public async Task<List<News>> GetTop5LatestNews()
        {
            return await _db.News.OrderByDescending(n => n.ModifiedDate).Take(5).ToListAsync();
        }
    }
}

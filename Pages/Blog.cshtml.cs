using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Group1_CourseOnline.Pages
{

	public class BlogModel : PageModel
	{


		public readonly SWP391_BlueEduContext _db;
		public BlogModel(SWP391_BlueEduContext db)
		{
			_db = db;
		}

		[BindProperty]
		public List<News> LatestNews { get; set; }
		public List<News> News { get; set; }
		public List<NewCategory> NewCategorys { get; set; }
		private int pageSize = 5;
		public decimal totalPage;
		public int Id;
		public int pageNumber;
		public async Task<IActionResult> OnGet(int id,string search,int pagenumber) // Thêm tham số id để truyền thông tin về tin tức được chọn

		{
			Id = id;
			
			var query = await _db.News.Include(c=>c.Employee).ToListAsync(); 
			if (id != 0)
			{
                var selectedCategory = await _db.NewCategories.FindAsync(id);
                if (selectedCategory != null)
                {
                    ViewData["Title"] = selectedCategory.Name; 
                    query = query.Where(c => c.CategoryNewId == id).ToList();
				}
            }
            else
            {
                ViewData["Title"] = "List Blog";
            }
            if (search != null)
			{
                ViewData["Title"] = "Search Results: " + search;
                query = query.Where(c => c.NewsTitle.Contains(search)).ToList();
            }
			if (query.Count() % pageSize != 0)
			{
				totalPage = query.Count() / pageSize + 1;

			}
			else
			{
				totalPage = query.Count() / pageSize;
			}
			if (pagenumber <= 0)
			{
				pagenumber = 1;
			}
			pageNumber = pagenumber;
            News = query.Skip((pagenumber - 1) * pageSize).Take(pageSize).ToList();
            if (News == null)
			{
				return Page(); // Trả về 404 Not Found nếu không tìm thấy tin tức
			}
			LatestNews = await GetTop3LatestNews();
           NewCategorys = _db.NewCategories.ToList();
			return Page();
		}

		public async Task<List<News>> GetTop3LatestNews()
		{
			return await _db.News.OrderByDescending(n => n.ModifiedDate).Take(3).ToListAsync();
		}
	}
}

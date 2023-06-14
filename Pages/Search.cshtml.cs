using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Group1_CourseOnline.Pages
{
    public class SearchModel : PageModel
    {
        public readonly SWP391_BlueEduContext _db;
        public SearchModel(SWP391_BlueEduContext db)
        { 
            _db = db;
        }
        [BindProperty(SupportsGet = true)]
        public string CategoryName { get; set; }
        [BindProperty]
        public List<Product> Products { get; set; }
        [BindProperty]
        public List<Category> Categories { get; set; }
        
        [BindProperty]
        public List<News> LatestNews { get; set; }
        public List<NewCategory> NewCategorys { get; set; }

        public List<News> News { get; set; }
        private int pageSize = 5;
        public decimal totalPage;
        public int catid;
        public int pageNumber;
        public string Search;

        public int a { get; set; }
        

        public void takeA(int b)
        {
            a = b;           
        }
        
        public async Task<IActionResult> OnGet(string search, int pagenumber)
        {
            Search= search; 
            Categories = _db.Categories.ToList();
            if (_db.Products != null)
            {
                var queryProduct = (from item in _db.Products
                                    join c in _db.Categories
                                    on item.CategoryId equals c.CategoryId
                                    where item.ProductName.Contains(search)
                                    select new
                                    {
                                        Cmt = (from cmt in _db.Comments where item.ProductId == cmt.ProductId select cmt).Count(),
                                        item.ProductId,
                                        item.UnitPrice,
                                        item.ProductName,
                                        item.Employee,
                                        item.Image,
                                        item.ProductDescription,
                                        c.CategoryId
                                    }).ToList();
          

                var query = await _db.News.Include(c => c.Employee).ToListAsync();

                if (search != null)
                {
                    ViewData["Title"] = "Search Results: " + search;
                    query = query.Where(c => c.NewsTitle.Contains(search)).ToList();
                }


                if ((query.Count() + queryProduct.Count()) % pageSize != 0)
                {
                    totalPage = (query.Count() + queryProduct.Count()) / pageSize + 1;

                }
                else
                {
                    totalPage = (query.Count() + queryProduct.Count()) / pageSize;
                }
                if (pagenumber <= 0)
                {
                    pagenumber = 1;
                }
                pageNumber = pagenumber;
                
                var products = queryProduct.Skip((pagenumber - 1) * pageSize).Take(pageSize).ToList();
                if (products.Count < pageSize && products.Count >0 ) {
                    News = query.Skip(0).Take(pageSize-products.Count()).ToList();
                     a = News.Count();
                    
                }
                takeA(a);
                if (products.Count == 0)
                {
                    News = query.Skip(a).Take(pageSize).ToList();
                }
                if (products.Count == pageSize)
                {
                    News = query.Take(0).ToList();               
                }
               
                

                
                
               
                
                LatestNews = await GetTop3LatestNews();
                NewCategorys = _db.NewCategories.ToList();

               
                ViewData["products"] = products;
            }
            return Page();
        }
        public async Task<List<News>> GetTop3LatestNews()
        {
            return await _db.News.OrderByDescending(n => n.ModifiedDate).Take(3).ToListAsync();
        }
    }
}

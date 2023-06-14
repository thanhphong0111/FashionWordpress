using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Group1_CourseOnline.Pages.Admin
{
    public class IndexModel : PageModel
    {
        public readonly SWP391_BlueEduContext _db;
        public IndexModel(SWP391_BlueEduContext db)
        {
            _db = db;
        }
        public int NewOrders { get; set; }
        public float PerOrder { get; set; }
        public int NewUser { get; set; }

        public float PerNewUser { get; set; }

        public decimal SalesRevenue { get; set; }

        public double PerSalesRevenue { get; set; }

        public decimal AvgRate { get; set; }
        public decimal ytdAvgRate { get; set; }

        public Dictionary<int, int> orders12months;
        public Dictionary<int, int> Oldorders12months;
        public Dictionary<int, int> Orders6Months;


        public Dictionary<int, float> profitOrders2022;
        public Dictionary<int, float> profitOrders2023;

        public Dictionary<int, float> profitOrders6Months;
        public double percentage { get; set; }
        public double incomePercentage { get; set; }
        public int TotalCus { get; set; }
        public int TotalSales { get; set; }
        public float TotalProfit { get; set; }
        public int TotalBlogView { get; set; }
        public int TotalProductView { get; set; }
        public int TotalComment { get; set; }
        public async Task<IActionResult> OnGet()
        {
            if (HttpContext.Session.GetString("admin") == null)
            {
                return RedirectToPage("/Login");
            }
            
            TotalCus = _db.Customers.GroupBy(e => e.CustomerId).Count();
            TotalProductView = _db.Products.Sum(e => e.Views);
            TotalBlogView = _db.News.Sum(e => e.Views);
            TotalComment = _db.Comments.Count();
            TotalSales = _db.Orders.Count();

            try
            {// Tạo một mảng chứa các tháng từ 1 đến 12




                int[] months = Enumerable.Range(1, 12).ToArray();

                // Truy vấn số đơn hàng từ tháng 1 đến tháng 12 năm 2023
                var orders12monthsQuery = from month in months
                                          join order in _db.Orders on new { Month = month, Year = 2023 } equals new { Month = order.OrderDate.Value.Month, Year = order.OrderDate.Value.Year } into orderGroup
                                          from order in orderGroup.DefaultIfEmpty()
                                          group order by month into orderMonth
                                          orderby orderMonth.Key ascending
                                          select new { Month = orderMonth.Key, Orders = orderMonth.Count(o => o != null) };

                orders12months = orders12monthsQuery.ToDictionary(e => e.Month, e => e.Orders);

                // Truy vấn số đơn hàng từ tháng 1 đến tháng 12 năm 2022
                var oldOrders12monthsQuery = from month in months
                                             join order in _db.Orders on new { Month = month, Year = 2022 } equals new { Month = order.OrderDate.Value.Month, Year = order.OrderDate.Value.Year } into orderGroup
                                             from order in orderGroup.DefaultIfEmpty()
                                             group order by month into orderMonth
                                             orderby orderMonth.Key ascending
                                             select new { Month = orderMonth.Key, Orders = orderMonth.Count(o => o != null) };

                Oldorders12months = oldOrders12monthsQuery.ToDictionary(e => e.Month, e => e.Orders);

                int totalOrders2023 = orders12months[DateTime.Now.Month];

                // Tính tổng số đơn hàng của năm 2022
                int totalOrders2022 = Oldorders12months[DateTime.Now.Month];

                // Tính phần trăm tương đối giữa hai năm
                 percentage = (double)(totalOrders2023 - totalOrders2022) / totalOrders2022 * 100;



                profitOrders6Months = Enumerable.Range(1, 6)
    .Select(month => new
    {
        Month = month,
        Total = _db.Orders.Join(_db.OrderDetails,
            o => o.OrderId,
            od => od.OrderId,
            (o, od) => new { Order = o, OrderDetails = od })
            .Where(x => x.Order.OrderDate.Value.Year == DateTime.Now.Year && x.Order.OrderDate.Value.Month == DateTime.Now.Month - (month - 1))
            .Sum(x => (float)x.OrderDetails.Quantity * (float)x.OrderDetails.UnitPrice)
    })
    .ToDictionary(e => e.Month, e => e.Total);


                var order6monthQuery = (
    from month in Enumerable.Range(DateTime.Now.AddMonths(-5).Month, 6)
    join order in _db.Orders on new { Month = month, Year = DateTime.Now.Year } equals new { Month = order.OrderDate.Value.Month, Year = order.OrderDate.Value.Year } into orderGroup
    from order in orderGroup.DefaultIfEmpty()
    group order by month into orderMonth
    orderby orderMonth.Key ascending
    select new { Month = orderMonth.Key, Orders = orderMonth.Count(o => o != null) }
);

                Orders6Months = order6monthQuery.ToDictionary(e => e.Month, e => e.Orders);



                profitOrders2022 = Enumerable.Range(1, 12).Select(month => new
                {
                    Month = month,
                    Total = _db.Orders.Join(_db.OrderDetails,
            o => o.OrderId,
            od => od.OrderId,
            (o, od) => new { Order = o, OrderDetails = od })
            .Where(x => x.Order.OrderDate.Value.Year == 2022 && x.Order.OrderDate.Value.Month == month)
            .Sum(x => (float)x.OrderDetails.Quantity * (float)x.OrderDetails.UnitPrice)

                }).ToDictionary(e => e.Month, e => e.Total);



                profitOrders2023 = Enumerable.Range(1, 12)
                   .Select(month => new
                   {
                       Month = month,
                       Total = _db.Orders.Join(_db.OrderDetails,
                           o => o.OrderId,
                           od => od.OrderId,
                           (o, od) => new { Order = o, OrderDetails = od })
                           .Where(x => x.Order.OrderDate.Value.Year == 2023 && x.Order.OrderDate.Value.Month == month)
                           .Sum(x => (float)x.OrderDetails.Quantity * (float)x.OrderDetails.UnitPrice)

                   })
                   .ToDictionary(e => e.Month, e => e.Total);

                float profit2023 = profitOrders2023[DateTime.Now.Month];
                float profit2022 = profitOrders2022[DateTime.Now.Month];
                TotalProfit = _db.OrderDetails.Sum(x => x.Quantity * (float)x.UnitPrice);
                incomePercentage = (profit2023 - profit2022) / profit2022 * 100;

                //compared number orders to yesterday
                NewOrders = _db.Orders.Where(c => c.OrderDate.Value.Date == DateTime.Now.Date).Count();
                int ytdOrder = _db.Orders.Where(c => c.OrderDate.Value.Date == DateTime.Now.AddDays(-1).Date).Count();
                PerOrder = (float)NewOrders / ytdOrder * 100;

                // compared new register to yesterday
                NewUser = _db.Accounts.Where(c => c.CreateDate.Value.Date == DateTime.Now.Date).Count();
                PerNewUser = (float)NewUser / (_db.Accounts.Where(c => c.CreateDate.Value.Date == DateTime.Now.AddDays(-1).Date).Count()) * 100;

                // compared SalesRevenue to yesterday
                SalesRevenue = _db.OrderDetails.Include(o => o.Order).Where(o => o.Order.OrderDate.Value.Date == DateTime.Now.Date).Sum(o => o.UnitPrice * o.Quantity);

                decimal ytdSalesRevenue = _db.OrderDetails.Include(o => o.Order).Where(o => o.Order.OrderDate.Value.Date == DateTime.Now.AddDays(-1).Date).Sum(o => o.UnitPrice * o.Quantity);

                double roundedNumber = (double)((SalesRevenue - ytdSalesRevenue) / ytdSalesRevenue * 100);
                PerSalesRevenue = Math.Round(roundedNumber, 2);
                // compared rating to yesterday
                var Rating = _db.Comments.Where(o => o.CommentTime.Date == DateTime.Now.Date).Select(o => o.Rating).ToList();

                AvgRate = (decimal)Rating.Average();
                AvgRate = Math.Round(AvgRate, 2);

                var ytdRating = _db.Comments.Where(o => o.CommentTime.Date == DateTime.Now.AddDays(-1).Date).Select(o => o.Rating).ToList();

                ytdAvgRate = (decimal)ytdRating.Average();
                ytdAvgRate = Math.Round(ytdAvgRate, 2);

                
            }
            catch (Exception ex)
            {
                return Page();
            }

            
            return Page();
            // compared Rating to yesterday
        }
    }
}

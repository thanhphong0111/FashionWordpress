using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Group1_CourseOnline.Pages.Admin.ReportStatic
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

        public Dictionary<string, int> orders12months;
        public Dictionary<string, int> Oldorders12months;
        public Dictionary<string, int> profitOrders6Months;
        public Dictionary<string, int> Orders6Months;
        //public Dictionary<int, int> Customer6Month;

        public Dictionary<string, int> Customer6Month;
        public Dictionary<string, int> Customer7Day;
        public Dictionary<string, int> CustomerYear;

        public Dictionary<string, float> profitOrders2022;
        public Dictionary<string, float> profitOrders2023;

        public Dictionary<string, int> ViewByCategory;
        public Dictionary<string, decimal> IncomeCategory;


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
            try
            {// Tạo một mảng chứa các tháng từ 1 đến 12


                var monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;

                int[] months = Enumerable.Range(1, 12).ToArray();

                // Truy vấn số đơn hàng từ tháng 1 đến tháng 12 năm 2023
                var orders12monthsQuery = from month in months
                                          join order in _db.Orders on new { Month = month, Year = 2023 } equals new { Month = order.OrderDate.Value.Month, Year = order.OrderDate.Value.Year } into orderGroup
                                          from order in orderGroup.DefaultIfEmpty()
                                          group order by month into orderMonth
                                          orderby orderMonth.Key ascending
                                          select new { Month = monthNames[orderMonth.Key - 1], Orders = orderMonth.Count(o => o != null) };

                orders12months = orders12monthsQuery.ToDictionary(e => e.Month, e => e.Orders);

                // Truy vấn số đơn hàng từ tháng 1 đến tháng 12 năm 2022
                var oldOrders12monthsQuery = from month in months
                                             join order in _db.Orders on new { Month = month, Year = 2022 } equals new { Month = order.OrderDate.Value.Month, Year = order.OrderDate.Value.Year } into orderGroup
                                             from order in orderGroup.DefaultIfEmpty()
                                             group order by month into orderMonth
                                             orderby orderMonth.Key ascending
                                             select new { Month = monthNames[orderMonth.Key - 1], Orders = orderMonth.Count(o => o != null) };

                Oldorders12months = oldOrders12monthsQuery.ToDictionary(e => e.Month, e => e.Orders);

                int totalOrders2023 = orders12months[GetMonthName(DateTime.Now.Month)];

                // Tính tổng số đơn hàng của năm 2022
                int totalOrders2022 = Oldorders12months[GetMonthName(DateTime.Now.Month)];

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
                .Sum(x => (int)(x.OrderDetails.Quantity * x.OrderDetails.UnitPrice))
        }).Reverse()
        .ToDictionary(e => monthNames[e.Month], e => e.Total);


                var order6monthQuery = (
    from month in Enumerable.Range(DateTime.Now.AddMonths(-5).Month, 6)
    join order in _db.Orders on new { Month = month, Year = DateTime.Now.Year } equals new { Month = order.OrderDate.Value.Month, Year = order.OrderDate.Value.Year } into orderGroup
    from order in orderGroup.DefaultIfEmpty()
    group order by month into orderMonth
    orderby orderMonth.Key ascending
    select new { Month = monthNames[orderMonth.Key - 1], Orders = orderMonth.Count(o => o != null) });


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

                }).ToDictionary(e => monthNames[e.Month-1], e => e.Total);



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
                   .ToDictionary(e => monthNames[e.Month-1], e => e.Total);

                float profit2023 = profitOrders2023[GetMonthName(DateTime.Now.Month )];
                float profit2022 = profitOrders2022[GetMonthName(DateTime.Now.Month )];
                TotalProfit = _db.OrderDetails.Sum(x => x.Quantity * (float)x.UnitPrice);
                incomePercentage = (profit2023 - profit2022) / profit2022 * 100;
                TotalSales = _db.Orders.Count();
                //compared number orders to yesterday

                var startDate = DateTime.Now.AddMonths(-5).Date;
                var endDate = DateTime.Now.Date;


                var cus6months = (
                    from month in Enumerable.Range(0, 6)
                    let targetDate = startDate.AddMonths(month)
                    join acc in _db.Accounts.Where(a => a.Role == 2 && a.CreateDate >= startDate && a.CreateDate <= endDate)
                        on targetDate.Month equals acc.CreateDate.Value.Month into cusGroup
                    from acc in cusGroup.DefaultIfEmpty()
                    group acc by targetDate.Month into createMonth
                    orderby createMonth.Key ascending
                    select new { Month = monthNames[createMonth.Key - 1], Customer = createMonth.Count() }
                );

                Customer6Month = cus6months.ToDictionary(e => (e.Month), e => e.Customer);

                Customer6Month.Add("", 0);
                var startday = DateTime.Now.AddDays(-6).Date;
                var endday = DateTime.Now.Date;
                var cusday = (from day in Enumerable.Range(0, 7)
                              let targetDate = startday.AddDays(day)
                              join acc in _db.Accounts.Where(a => a.Role == 2 && a.CreateDate >= startday && a.CreateDate <= endday)
                                  on targetDate.Date equals acc.CreateDate.Value.Date into cusGroup
                              from acc in cusGroup.DefaultIfEmpty()
                              group acc by targetDate.Date into createDate
                              orderby createDate.Key ascending
                              select new { Day = createDate.Key.ToString("dd/MM"), Customer = createDate.Count() });

                Customer7Day = cusday.ToDictionary(e => e.Day, e => e.Customer);

                Customer7Day.Add("", 0);

                var cusYear = (
      from acc in _db.Accounts.Where(a => a.Role == 2 && a.CreateDate.HasValue)
      group acc by acc.CreateDate.Value.Year into createYear
      orderby createYear.Key ascending
      select new { Year = createYear.Key.ToString(), Customer = createYear.Count() }
  );

                CustomerYear = cusYear.ToDictionary(e => e.Year, e => e.Customer);
                CustomerYear.Add("", 0);

                ViewByCategory = _db.Categories
       .Join(_db.Products,
           category => category.CategoryId,
           product => product.CategoryId,
           (category, product) => new { category.CategoryName, product.Views })
       .GroupBy(result => result.CategoryName)
       .Select(group => new { CategoryName = group.Key, TotalViews = group.Sum(item => item.Views) })
       .ToDictionary(result => result.CategoryName, result => result.TotalViews);


                IncomeCategory = _db.OrderDetails
    .Where(od => od.Product.CategoryId.HasValue) // Chỉ lấy những order detail có Product.CategoryId khác null
    .GroupBy(od => od.Product.Category.CategoryName)
    .Select(group => new { CategoryName = group.Key, TotalRevenue = group.Sum(od => od.UnitPrice * od.Quantity) })
    .ToDictionary(result => result.CategoryName, result => result.TotalRevenue);


            }
            catch (Exception ex)
            {
                return Page();
            }


            return Page();
        }
        private string GetMonthName(int month)
        {
            var cultureInfo = CultureInfo.CreateSpecificCulture("en-US");
            return cultureInfo.DateTimeFormat.GetMonthName(month);
        }
    }
}

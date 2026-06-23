using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MobileShopOrderMangement.Models;

namespace MobileShopOrderMangement.Controllers
{
    public class HomeController : Controller
    {
        private ProductOrderDBEntities db = new ProductOrderDBEntities();

        public ActionResult Index()
        {
            // Get Business Summary Statistics
            var businessSummary = new Dictionary<string, object>();
            
            try
            {
                // Total Products
                businessSummary["TotalProducts"] = db.Products.Count();
                businessSummary["ActiveProducts"] = db.Products.Where(p => p.IsActive).Count();
                
                // Total Orders
                businessSummary["TotalOrders"] = db.Orders.Count();
                
                // Total Revenue
                businessSummary["TotalRevenue"] = db.Orders.Sum(o => (decimal?)o.TotalAmount) ?? 0;
                
                // Total Customers
                businessSummary["TotalCustomers"] = db.Customers.Count();
                
                // Recent Orders Count (Last 7 days)
                var sevenDaysAgo = DateTime.Now.AddDays(-7);
                businessSummary["RecentOrders"] = db.Orders.Where(o => o.OrderDate >= sevenDaysAgo).Count();
                
                // Average Order Value
                var orderCount = db.Orders.Count();
                businessSummary["AverageOrderValue"] = orderCount > 0 ? (businessSummary["TotalRevenue"] as decimal?) / orderCount : 0;
                
                // Top Categories (by product count)
                var topCategories = db.ProductCategories
                    .Select(c => new { 
                        CategoryName = c.CategoryName, 
                        ProductCount = c.Products.Count() 
                    })
                    .OrderByDescending(c => c.ProductCount)
                    .Take(5)
                    .ToList();
                businessSummary["TopCategories"] = topCategories;
                
                // Monthly Revenue Trend (last 6 months)
                var sixMonthsAgo = DateTime.Now.AddMonths(-6);
                var monthlyRevenue = db.Orders
                    .Where(o => o.OrderDate >= sixMonthsAgo)
                    .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                    .Select(g => new { 
                        YearMonth = new DateTime(g.Key.Year, g.Key.Month, 1), 
                        Revenue = g.Sum(o => o.TotalAmount) 
                    })
                    .OrderBy(m => m.YearMonth)
                    .ToList();
                businessSummary["MonthlyRevenue"] = monthlyRevenue;
            }
            catch
            {
                // If database is not available, provide default values
                businessSummary["TotalProducts"] = 0;
                businessSummary["ActiveProducts"] = 0;
                businessSummary["TotalOrders"] = 0;
                businessSummary["TotalRevenue"] = 0;
                businessSummary["TotalCustomers"] = 0;
                businessSummary["RecentOrders"] = 0;
                businessSummary["AverageOrderValue"] = 0;
                businessSummary["TopCategories"] = new List<dynamic>();
                businessSummary["MonthlyRevenue"] = new List<dynamic>();
            }
            
            ViewBag.BusinessSummary = businessSummary;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
using MobileShopOrderMangement.Models;
using MobileShopOrderMangement.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace MobileShopOrderMangement.Controllers
{
    public class OrderController : Controller
    {
        private ProductOrderDBEntities db = new ProductOrderDBEntities();

        // GET: Order
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderMasterViewModel
                {
                    OrderId = o.OrderId,
                    CustomerName = o.Customer.CustomerName,
                    ContactNumber = o.Customer.ContactNumber,
                    ContactAddress = o.Customer.ContactAddress,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    CustomerId = o.CustomerId
                }).ToList();

            return View(orders);
        }

        // GET: Order/Create
        public ActionResult Create()
        {
            ViewBag.ProductCategories = new SelectList(db.ProductCategories, "ProductCategoryId", "CategoryName");
            return View(new OrderMasterViewModel());
        }

        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OrderMasterViewModel model, string OrderDetailsListJson)
        {
            // Deserialize JSON order details
            if (!string.IsNullOrEmpty(OrderDetailsListJson))
            {
                model.OrderDetailsList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OrderDetailsViewModel>>(OrderDetailsListJson);
            }

            if (ModelState.IsValid)
            {
                if (model.OrderDetailsList == null || !model.OrderDetailsList.Any())
                {
                    TempData["ErrorMessage"] = "Please add at least one product to the order.";
                    ViewBag.ProductCategories = new SelectList(db.ProductCategories, "ProductCategoryId", "CategoryName");
                    return View(model);
                }

                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        // Validate stock availability
                        foreach (var item in model.OrderDetailsList)
                        {
                            var product = db.Products.Find(item.ProductId);
                            if (product.AvailableQuantity < item.OrderQuantity)
                            {
                                TempData["ErrorMessage"] = $"Insufficient stock for {product.ProductName}. Available: {product.AvailableQuantity}";
                                ViewBag.ProductCategories = new SelectList(db.ProductCategories, "ProductCategoryId", "CategoryName");
                                return View(model);
                            }
                        }

                        // Create or find customer
                        var customer = db.Customers.FirstOrDefault(c => c.ContactNumber == model.ContactNumber);
                        if (customer == null)
                        {
                            customer = new Customer
                            {
                                CustomerName = model.CustomerName,
                                ContactNumber = model.ContactNumber,
                                ContactAddress = model.ContactAddress
                            };
                            db.Customers.Add(customer);
                            db.SaveChanges();
                        }
                        else
                        {
                            // Update customer info
                            customer.CustomerName = model.CustomerName;
                            customer.ContactAddress = model.ContactAddress;
                            db.Entry(customer).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        // Create order
                        var order = new Order
                        {
                            CustomerId = customer.CustomerId,
                            OrderDate = DateTime.Now,
                            TotalAmount = model.OrderDetailsList.Sum(d => d.Amount)
                        };
                        db.Orders.Add(order);
                        db.SaveChanges();

                        // Create order details and update product quantity
                        foreach (var item in model.OrderDetailsList)
                        {
                            var orderDetail = new OrderDetail
                            {
                                OrderId = order.OrderId,
                                ProductCategoryId = item.ProductCategoryId,
                                ProductId = item.ProductId,
                                OrderQuantity = item.OrderQuantity,
                                OrderUnit = item.OrderUnit,
                                UnitPrice = item.UnitPrice,
                                Amount = item.Amount
                            };
                            db.OrderDetails.Add(orderDetail);

                            // Update product quantity
                            var product = db.Products.Find(item.ProductId);
                            product.AvailableQuantity -= item.OrderQuantity;
                            db.Entry(product).State = EntityState.Modified;
                        }

                        db.SaveChanges();
                        transaction.Complete();

                        TempData["SuccessMessage"] = "Order placed successfully!";
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = "Error placing order: " + ex.Message;
                        ViewBag.ProductCategories = new SelectList(db.ProductCategories, "ProductCategoryId", "CategoryName");
                        return View(model);
                    }
                }
            }

            ViewBag.ProductCategories = new SelectList(db.ProductCategories, "ProductCategoryId", "CategoryName");
            return View(model);
        }

        // GET: Order/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = db.Orders.Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            var model = new OrderMasterViewModel
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer.CustomerName,
                ContactNumber = order.Customer.ContactNumber,
                ContactAddress = order.Customer.ContactAddress,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderDetailsList = order.OrderDetails.Select(od => new OrderDetailsViewModel
                {
                    OrderDetailsId = od.OrderDetailsId,
                    OrderId = od.OrderId,
                    ProductCategoryId = od.ProductCategoryId,
                    ProductId = od.ProductId,
                    OrderQuantity = od.OrderQuantity,
                    OrderUnit = od.OrderUnit,
                    UnitPrice = od.UnitPrice,
                    Amount = od.Amount,
                    ProductName = od.Product.ProductName,
                    CategoryName = od.ProductCategory.CategoryName
                }).ToList()
            };

            ViewBag.ProductCategories = new SelectList(db.ProductCategories, "ProductCategoryId", "CategoryName");
            return View(model);
        }

        // POST: Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OrderMasterViewModel model, string OrderDetailsListJson)
        {
            // Deserialize JSON order details
            if (!string.IsNullOrEmpty(OrderDetailsListJson))
            {
                model.OrderDetailsList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OrderDetailsViewModel>>(OrderDetailsListJson);
            }

            if (ModelState.IsValid)
            {
                if (model.OrderDetailsList == null || !model.OrderDetailsList.Any())
                {
                    TempData["ErrorMessage"] = "Please add at least one product to the order.";
                    ViewBag.ProductCategories = new SelectList(db.ProductCategories, "ProductCategoryId", "CategoryName");
                    return View(model);
                }

                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        var order = db.Orders.Include(o => o.OrderDetails).FirstOrDefault(o => o.OrderId == model.OrderId);
                        if (order == null)
                        {
                            return HttpNotFound();
                        }

                        // Restore original product quantities
                        foreach (var oldDetail in order.OrderDetails.ToList())
                        {
                            var product = db.Products.Find(oldDetail.ProductId);
                            product.AvailableQuantity += oldDetail.OrderQuantity;
                            db.Entry(product).State = EntityState.Modified;
                        }

                        // Validate stock availability for new quantities
                        foreach (var item in model.OrderDetailsList)
                        {
                            var product = db.Products.Find(item.ProductId);
                            if (product.AvailableQuantity < item.OrderQuantity)
                            {
                                // Restore quantities before returning error
                                foreach (var oldDetail in order.OrderDetails)
                                {
                                    var prod = db.Products.Find(oldDetail.ProductId);
                                    prod.AvailableQuantity -= oldDetail.OrderQuantity;
                                    db.Entry(prod).State = EntityState.Modified;
                                }
                                db.SaveChanges();

                                TempData["ErrorMessage"] = $"Insufficient stock for {product.ProductName}. Available: {product.AvailableQuantity}";
                                ViewBag.ProductCategories = new SelectList(db.ProductCategories, "ProductCategoryId", "CategoryName");
                                return View(model);
                            }
                        }

                        // Update customer
                        var customer = db.Customers.Find(order.CustomerId);
                        customer.CustomerName = model.CustomerName;
                        customer.ContactNumber = model.ContactNumber;
                        customer.ContactAddress = model.ContactAddress;
                        db.Entry(customer).State = EntityState.Modified;

                        // Delete old order details
                        db.OrderDetails.RemoveRange(order.OrderDetails);

                        // Create new order details and update product quantities
                        foreach (var item in model.OrderDetailsList)
                        {
                            var orderDetail = new OrderDetail
                            {
                                OrderId = order.OrderId,
                                ProductCategoryId = item.ProductCategoryId,
                                ProductId = item.ProductId,
                                OrderQuantity = item.OrderQuantity,
                                OrderUnit = item.OrderUnit,
                                UnitPrice = item.UnitPrice,
                                Amount = item.Amount
                            };
                            db.OrderDetails.Add(orderDetail);

                            // Update product quantity
                            var product = db.Products.Find(item.ProductId);
                            product.AvailableQuantity -= item.OrderQuantity;
                            db.Entry(product).State = EntityState.Modified;
                        }

                        // Update order
                        order.TotalAmount = model.OrderDetailsList.Sum(d => d.Amount);
                        db.Entry(order).State = EntityState.Modified;

                        db.SaveChanges();
                        transaction.Complete();

                        TempData["SuccessMessage"] = "Order updated successfully!";
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = "Error updating order: " + ex.Message;
                        ViewBag.ProductCategories = new SelectList(db.ProductCategories, "ProductCategoryId", "CategoryName");
                        return View(model);
                    }
                }
            }

            ViewBag.ProductCategories = new SelectList(db.ProductCategories, "ProductCategoryId", "CategoryName");
            return View(model);
        }

        // GET: Order/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = db.Orders.Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            var model = new OrderMasterViewModel
            {
                OrderId = order.OrderId,
                CustomerName = order.Customer.CustomerName,
                ContactNumber = order.Customer.ContactNumber,
                ContactAddress = order.Customer.ContactAddress,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderDetailsList = order.OrderDetails.Select(od => new OrderDetailsViewModel
                {
                    ProductName = od.Product.ProductName,
                    CategoryName = od.ProductCategory.CategoryName,
                    OrderQuantity = od.OrderQuantity,
                    OrderUnit = od.OrderUnit,
                    UnitPrice = od.UnitPrice,
                    Amount = od.Amount
                }).ToList()
            };

            return View(model);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    var order = db.Orders.Include(o => o.OrderDetails).FirstOrDefault(o => o.OrderId == id);
                    if (order == null)
                    {
                        return HttpNotFound();
                    }

                    // Restore product quantities
                    foreach (var detail in order.OrderDetails)
                    {
                        var product = db.Products.Find(detail.ProductId);
                        product.AvailableQuantity += detail.OrderQuantity;
                        db.Entry(product).State = EntityState.Modified;
                    }

                    db.Orders.Remove(order);
                    db.SaveChanges();
                    transaction.Complete();

                    TempData["SuccessMessage"] = "Order deleted successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error deleting order: " + ex.Message;
                    return RedirectToAction("Index");
                }
            }
        }

        // AJAX: Get Product Details
        [HttpGet]
        public JsonResult GetProductDetails(int productId)
        {
            var product = db.Products.Find(productId);
            if (product == null)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                success = true,
                unit = product.Unit,
                unitPrice = product.UnitPrice,
                availableQuantity = product.AvailableQuantity
            }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

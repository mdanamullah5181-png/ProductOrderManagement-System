using MobileShopOrderMangement.Models;
using MobileShopOrderMangement.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MobileShopOrderMangement.Controllers
{
    public class ProductController : Controller
    {
        private ProductOrderDBEntities db = new ProductOrderDBEntities();

        // GET: Product
        public ActionResult Index(int? categoryId, string searchName)
        {
            var products = db.Products.Include(p => p.ProductCategory).AsQueryable();

            // Filter by Category
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                products = products.Where(p => p.ProductCategoryId == categoryId.Value);
            }

            // Filter by Product Name
            if (!string.IsNullOrEmpty(searchName))
            {
                products = products.Where(p => p.ProductName.Contains(searchName));
            }

            var productList = products.Select(p => new ProductViewModel
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Unit = p.Unit,
                UnitPrice = p.UnitPrice,
                AvailableQuantity = p.AvailableQuantity,
                ProductImage = p.ProductImage,
                ProductCategoryId = p.ProductCategoryId,
                CategoryName = p.ProductCategory.CategoryName,
                IsActive = p.IsActive
            }).ToList();

            // Send categories to view
            ViewBag.Categories = new SelectList(db.ProductCategories, "ProductCategoryId", "CategoryName");
            ViewBag.SelectedCategory = categoryId;
            ViewBag.SearchName = searchName;

            return View(productList);
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            ViewBag.ProductCategoryId = new SelectList(db.ProductCategories, "ProductCategoryId", "CategoryName");
            ViewBag.Units = new SelectList(new[] { "Piece", "kg", "g", "Liter", "ml", "Box", "Dozen", "Pack", "Bundle", "Meter", "Cm", "Hour", "Day", "Month" });
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    ProductName = model.ProductName,
                    Unit = model.Unit,
                    UnitPrice = model.UnitPrice,
                    AvailableQuantity = model.AvailableQuantity,
                    IsActive = model.IsActive,
                    ProductCategoryId = model.ProductCategoryId
                };
                // Handle image upload
                if (model.ImageFile != null && model.ImageFile.ContentLength > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    model.ImageFile.SaveAs(path);
                    product.ProductImage = fileName;
                }
                db.Products.Add(product);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Product created successfully!";
                return RedirectToAction("Index");
            }
            ViewBag.ProductCategoryId = new SelectList(db.ProductCategories, "ProductCategoryId", "CategoryName", model.ProductCategoryId);
            ViewBag.Units = new SelectList(new[] { "Piece", "kg", "g", "Liter", "ml", "Box", "Dozen", "Pack", "Bundle", "Meter", "Cm", "Hour", "Day", "Month" }, model.Unit);
            return View(model);
        }

        // GET: Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            var model = new ProductViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Unit = product.Unit,
                UnitPrice = product.UnitPrice,
                AvailableQuantity = product.AvailableQuantity,
                ProductImage = product.ProductImage,
                ProductCategoryId = product.ProductCategoryId,
                IsActive = product.IsActive 
            };
            ViewBag.ProductCategoryId = new SelectList(db.ProductCategories, "ProductCategoryId", "CategoryName", model.ProductCategoryId);
            ViewBag.Units = new SelectList(new[] { "Piece", "kg", "g", "Liter", "ml", "Box", "Dozen", "Pack", "Bundle", "Meter", "Cm", "Hour", "Day", "Month" }, model.Unit);
            return View(model);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = db.Products.Find(model.ProductId);
                if (product == null)
                {
                    return HttpNotFound();
                }
                product.ProductName = model.ProductName;
                product.Unit = model.Unit;
                product.UnitPrice = model.UnitPrice;
                product.AvailableQuantity = model.AvailableQuantity;
                product.ProductCategoryId = model.ProductCategoryId.Value;
                product.IsActive = model.IsActive;

                // Handle image upload
                if (model.ImageFile != null && model.ImageFile.ContentLength > 0)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(product.ProductImage))
                    {
                        string oldImagePath = Path.Combine(Server.MapPath("~/Images"), product.ProductImage);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    // Save new image
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    model.ImageFile.SaveAs(path);
                    product.ProductImage = fileName;
                }
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Product updated successfully!";
                return RedirectToAction("Index");
            }
            ViewBag.ProductCategoryId = new SelectList(db.ProductCategories, "ProductCategoryId", "CategoryName", model.ProductCategoryId);
            return View(model);
        }

        // GET: Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var product = db.Products.Include(p => p.ProductCategory).FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            var model = new ProductViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Unit = product.Unit,
                UnitPrice = product.UnitPrice,
                AvailableQuantity = product.AvailableQuantity,
                ProductImage = product.ProductImage,
                ProductCategoryId = product.ProductCategoryId,
                CategoryName = product.ProductCategory.CategoryName
            };
            return View(model);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var product = db.Products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                // Delete image if exists
                if (!string.IsNullOrEmpty(product.ProductImage))
                {
                    string imagePath = Path.Combine(Server.MapPath("~/Images"), product.ProductImage);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                db.Products.Remove(product);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Product deleted successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Cannot delete this product. It may have related orders.";
                return RedirectToAction("Index");
            }
        }

        // AJAX: Get Products by Category
        [HttpGet]
        public JsonResult GetProductsByCategory(int categoryId)
        {
            var products = db.Products
                .Where(p => p.ProductCategoryId == categoryId)
                .Select(p => new
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Unit = p.Unit,
                    UnitPrice = p.UnitPrice,
                    AvailableQuantity = p.AvailableQuantity
                }).ToList();
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllCategories()
        {
            var categories = db.ProductCategories
                .Select(c => new
                {
                    ProductCategoryId = c.ProductCategoryId,
                    CategoryName = c.CategoryName
                }).ToList();
            return Json(categories, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public JsonResult ToggleStatus(int id)
        {
            try
            {
                var product = db.Products.Find(id);
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found" });
                }

                product.IsActive = !product.IsActive;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true, isActive = product.IsActive });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

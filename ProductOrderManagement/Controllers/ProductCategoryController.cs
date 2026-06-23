using MobileShopOrderMangement.Models;
using MobileShopOrderMangement.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MobileShopOrderMangement.Controllers
{
    public class ProductCategoryController : Controller
    {
        private ProductOrderDBEntities db = new ProductOrderDBEntities();

        // GET: ProductCategory
        public ActionResult Index()
        {
            var categories = db.ProductCategories.Select(c => new ProductCategoryViewModel
            {
                ProductCategoryId = c.ProductCategoryId,
                CategoryName = c.CategoryName,
                CategoryDescription = c.CategoryDescription
            }).ToList();

            return View(categories);
        }

        // GET: ProductCategory/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = new ProductCategory
                {
                    CategoryName = model.CategoryName,
                    CategoryDescription = model.CategoryDescription
                };

                db.ProductCategories.Add(category);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Product Category created successfully!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: ProductCategory/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var category = db.ProductCategories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            var model = new ProductCategoryViewModel
            {
                ProductCategoryId = category.ProductCategoryId,
                CategoryName = category.CategoryName,
                CategoryDescription = category.CategoryDescription
            };

            return View(model);
        }

        // POST: ProductCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = db.ProductCategories.Find(model.ProductCategoryId);
                if (category == null)
                {
                    return HttpNotFound();
                }

                category.CategoryName = model.CategoryName;
                category.CategoryDescription = model.CategoryDescription;

                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();

                TempData["SuccessMessage"] = "Product Category updated successfully!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: ProductCategory/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var category = db.ProductCategories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            var model = new ProductCategoryViewModel
            {
                ProductCategoryId = category.ProductCategoryId,
                CategoryName = category.CategoryName,
                CategoryDescription = category.CategoryDescription
            };

            return View(model);
        }

        // POST: ProductCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var category = db.ProductCategories.Find(id);
                if (category == null)
                {
                    return HttpNotFound();
                }

                db.ProductCategories.Remove(category);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Product Category deleted successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Cannot delete this category. It may have related products.";
                return RedirectToAction("Index");
            }
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

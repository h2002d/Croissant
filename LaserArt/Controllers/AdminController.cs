using LaserArt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LaserArt.Controllers
{
    public class AdminController : Controller
    {
        public AdminController()
        {
            Dictionary<Models.ParentCategory, List<Models.Category>> mainParent =
               new Dictionary<Models.ParentCategory, List<Models.Category>>();
            // ViewBag.ParentCategories = 
            var parents = LaserArt.Models.ParentCategory.GetCategories(null);
            foreach (var parent in parents)
            {
                var categories = LaserArt.Models.Category.GetCategoriesByParentId(parent.Id);
                mainParent.Add(parent, categories);
            }
            ViewBag.ParentCategories = mainParent;
        }
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Order()
        {
            var model=Models.Order.GetOrderById(null);
            return View(model);
        }
        public ActionResult OrderDetails(int OrderId)
        {
            
            var product = Product.GetProductsByOrderId(OrderId);
            return View(product);
        }
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [ValidateInput(false)]
        public ActionResult CreateCategory()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [ValidateInput(false)]
        public ActionResult EditCategory(int id)
        {
            ParentCategory category = Models.ParentCategory.GetCategories(id).FirstOrDefault();
            return View("CreateCategory", category);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [ValidateInput(false)]
        public ActionResult DeleteCategory(int id)
        {
            Models.ParentCategory.DeleteCategory(id);
            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult CreateCategory(ParentCategory newCategory)
        {
            try
            {
                newCategory.SaveCategory();
                return RedirectToAction("Index","Home");
            }
            catch (Exception ex)
            {
                return RedirectToAction("CreateCategory");
            }
        }
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult SetParent(int categoryId,int parentId)
        {
            try
            {
                Category.SetCategoryParent(categoryId, parentId);
                return Json("Ավելացված է", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Ձախողված է",JsonRequestBehavior.AllowGet);
            }
        }
    }
}
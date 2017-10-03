using LaserArt.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace LaserArt.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
            Dictionary<Models.ParentCategory, List<Models.Category>> mainParent = 
                new Dictionary<Models.ParentCategory, List<Models.Category>>();
           // ViewBag.ParentCategories = 
                var parents= LaserArt.Models.ParentCategory.GetCategories(null);
            foreach(var parent in parents)
            {
                var categories = LaserArt.Models.Category.GetCategoriesByParentId(parent.Id);
                mainParent.Add(parent, categories);
            }
            ViewBag.ParentCategories = mainParent;
        }
        public ActionResult Index()
        {
           
            //var recomended = LaserArt.Models.Product.GetProducts(null).Take(3).ToList();
            //categoryList.Add("Re:Store-Family рекомендует:", recomended);
            var categories = Models.Category.GetCategories(null);
           
           // ViewBag.Sales = Models.Sales.GetSalesById(null);
            return View(categories);
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
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult CreateProduct()
        {
            ViewBag.Categories = LaserArt.Models.Category.GetCategories(null);
            return View();
        }
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [ValidateInput(false)]
        public ActionResult EditProduct(int id)
        {
            Product product = Models.Product.GetProducts(id).FirstOrDefault();
            ViewBag.Categories = LaserArt.Models.Category.GetCategories(null);
            return View("CreateProduct", product);
        }
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateInput(false)]
        public ActionResult CreateProduct(Product newProduct)
        {
            try
            {
                newProduct.ProductDescription = "";
                newProduct.ImageSource1 = "";
                newProduct.ImageSource2 = "";
                newProduct.ImageSource3 = "";
                newProduct.PriceDiscounted = 0;

                newProduct.SaveProduct();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("CreateProduct");
            }
        }
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [ValidateInput(false)]
        public ActionResult CreateCategory()
        {
            ViewBag.ParentCategory = Models.ParentCategory.GetCategories(null);
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [ValidateInput(false)]
        public ActionResult EditCategory(int id)
        {
            ViewBag.ParentCategory = Models.ParentCategory.GetCategories(null);

            Category category = Models.Category.GetCategories(id).FirstOrDefault();
            return View("CreateCategory", category);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [ValidateInput(false)]
        public ActionResult DeleteCategory(int id)
        {
            Models.Category.DeleteCategory(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult CreateCategory(Category newCategory)
        {
            try
            {
                newCategory.SaveCategory();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("CreateCategory");
            }
        }
        public ActionResult Product(int id)
        {
            var product = LaserArt.Models.Product.GetProducts(id).FirstOrDefault();
            return View(product);
        }

        public ActionResult Category(int id)
        {
            var category = LaserArt.Models.Category.GetCategories(id).FirstOrDefault();
            ViewBag.CategoryName = category.CategoryName;
            ViewBag.CategoryId = id;
            var products = LaserArt.Models.Product.GetProductsByCategoryId(id);
            return View(products);
        }

        public ActionResult ParentCategory(int id)
        {
            var category = LaserArt.Models.ParentCategory.GetCategories(id).FirstOrDefault();
            ViewBag.CategoryName = category.Name;
            ViewBag.CategoryId = id;
            var products = LaserArt.Models.Category.GetCategoriesByParentId(id);
            return View(products);
        }

        [HttpPost]
        public JsonResult FileUpload()
        {
            HttpPostedFile file = null;
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                file = System.Web.HttpContext.Current.Request.Files["HttpPostedFileBase"];
            }
            string pic = System.IO.Path.GetFileName(file.FileName);
            string path = System.IO.Path.Combine(
                                   Server.MapPath("~/images"), pic);
            // file is uploaded
            file.SaveAs(path);

            // save the image path path to the database or you can send image 
            // directly to database
            // in-case if you want to store byte[] ie. for DB
            using (MemoryStream ms = new MemoryStream())
            {
                file.InputStream.CopyTo(ms);
                byte[] array = ms.GetBuffer();
            }


            // after successfully uploading redirect the user
            return Json("Картинка загружена", JsonRequestBehavior.AllowGet);
        }

      
      

       
       
        

        [HttpPost]
        public ActionResult RemoveProduct(int id)
        {
            Models.Product.DeleteProduct(id);
            return Json("Телефон удален.",JsonRequestBehavior.AllowGet);
        }

        public ActionResult Order(int id)
        {
           
                Session["ProductId"] = id;
            return PartialView("_PartialOrderDetails");
        }

        
       

        [HttpPost]
        public ActionResult OrderDetails(Order newOrder)
        {
            int id = Convert.ToInt32(Session["ProductId"]);
            
            CardModel model = new CardModel();
            model.product = Models.Product.GetProducts(id).First();
            model.ProductId = id;
            model.ProductQuantity = newOrder.Quantity;
            newOrder.Products.Clear();
            newOrder.Products.Add(model);
            newOrder.Id = newOrder.saveOrder();
            SendMail(newOrder);
            return RedirectToAction("OrderAproval", new { id = newOrder.Id });
        }
        public ActionResult OrderAproval(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        [HttpGet]
        public ActionResult OrderDetails(int? id)
        {
            return PartialView();
        }

        public void SendMail(Order newOrder)
        {
            try
            {
                string EmailTo = WebConfigurationManager.AppSettings["EmailTo"];
                string EmailFrom = WebConfigurationManager.AppSettings["EmailFrom"];
                string EmailFromPassword = WebConfigurationManager.AppSettings["EmailFromPassword"];

                MailMessage mail = new MailMessage();
                mail.To.Add(EmailTo);
                mail.From = new MailAddress(EmailFrom);
                mail.Subject = "ՆՈՐ ՊԱՏՎԵՐ!";
                StringBuilder Body = new StringBuilder();
                Body.Append("<b>Դուք ունեք նոր պատվեր</b><br/>")
                    .Append(string.Format("Պատվերի համարը:{0} <br/>", newOrder.Id))
                    .Append(string.Format("Հասցեն:{0} <br/>", newOrder.Address))

                    .Append(string.Format("Պատվիրատուի անունը:{0}<br/>", newOrder.Name));
                    foreach (var item in newOrder.Products) {
                   var product= Models.Product.GetProducts(item.ProductId).FirstOrDefault();
                    Body.Append(string.Format("Название товара:N{0} {1} <br/>", item.ProductId, product.ProductTitle));
                        }
               // Body.Append(string.Format("Вреия заказа: {0}", DateTime.Now));
                mail.Body = Body.ToString();
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(EmailFrom, EmailFromPassword); // Enter seders User name and password   
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult Search(string query)
        {
            var listProducts = Models.Product.GetProductsByQuery(query);
            ViewBag.Sales = Models.Sales.GetSalesById(null);
            ViewBag.Info = "Результаты поиска:";
            return View(listProducts);
        }
    }
}
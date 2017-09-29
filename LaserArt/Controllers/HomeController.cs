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
           
            ViewBag.Categories = LaserArt.Models.Category.GetCategories(null);
        }
        public ActionResult Index()
        {
            Dictionary<string, List<Product>> categoryList = new Dictionary<string, List<Models.Product>>();
           
            //var recomended = LaserArt.Models.Product.GetProducts(null).Take(3).ToList();
            //categoryList.Add("Re:Store-Family рекомендует:", recomended);
            var categories = Models.Category.GetCategories(null);
            foreach(var category in categories)
            {
                var productsList = Models.Product.GetProductsByCategoryId(Convert.ToInt32(category.Id)).Take(6).ToList();
                categoryList.Add(category.CategoryName, productsList);
            }
           // ViewBag.Sales = Models.Sales.GetSalesById(null);
            return View(categoryList);
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
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [ValidateInput(false)]
        public ActionResult EditCategory(int id)
        {
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

        public ActionResult Order(int? id)
        {
            List<CardModel> productList = new List<CardModel>();
            decimal sum = 0;
            if (id == null)
            {

                if (Request.Cookies["Cart"] != null)
                {
                    HttpCookie cookieOld = Request.Cookies["Cart"];

                    var value = cookieOld.Value;

                    var idList = JsonConvert.DeserializeObject<List<CardModel>>(value);
                    foreach (var item in idList)
                    {

                        item.product = LaserArt.Models.Product.GetProducts(Convert.ToInt32(item.ProductId)).FirstOrDefault();
                        productList.Add(item);
                        sum += item.ProductQuantity * item.product.Price;

                    }

                }
            }
            else
            {
                ViewBag.ListInt = id;
                CardModel model = new CardModel();
                model.product = LaserArt.Models.Product.GetProducts(id).FirstOrDefault();
                model.ProductId = Convert.ToInt32(id);
                model.ProductQuantity = 1;
                productList.Add(model);
                sum += model.product.Price;
            }
            ViewBag.Sum = sum;
            Models.Order order = new Models.Order();
            order.Products = productList;
            return PartialView("_PartialOrderDetails", order);
        }

        
       

        [HttpPost]
        public ActionResult OrderDetails(Order newOrder)
        {
            newOrder.Id= newOrder.saveOrder();
            SendMail(newOrder);
            return Json("Պատվերը կատարված է մենք կկապվենք ձեր հետ հաշված րոպեների ընթացքում",JsonRequestBehavior.AllowGet);
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
                mail.Subject = "НОВЫЙ ЗАКАЗ!";
                StringBuilder Body = new StringBuilder();
                Body.Append("<b>У вас есть новый заказ</b><br/>")
                    .Append(string.Format("Номер заказа:{0} <br/>", newOrder.Id))
                    .Append(string.Format("Адрес:{0} <br/>", newOrder.Address))

                    .Append(string.Format("Имя зказчика:{0} {1} <br/>", newOrder.Name));
                    foreach (var item in newOrder.Products) {
                   var product= Models.Product.GetProducts(item.ProductId).FirstOrDefault();
                    Body.Append(string.Format("Название товара:N{0} {1} <br/>", item.ProductId, product.ProductTitle));
                        }
                Body.Append(string.Format("Вреия заказа: {0}", DateTime.Now));
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
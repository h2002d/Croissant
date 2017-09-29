using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace LaserArt.Controllers
{
    public class MailController : Controller
    {
        // GET: Mail
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SendMail(int orderId)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add("davit.harutyunyan1996@gmail.com");
            mail.From = new MailAddress("softsonicip@gmail.com");
            mail.Subject = "Order";
            string Body = "Hello";
            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("softsonicip@gmail.com", "d341044*"); // Enter seders User name and password   
            smtp.EnableSsl = true;
            smtp.Send(mail);
            return Json("Картинка загружена", JsonRequestBehavior.AllowGet);
        }
    }
}
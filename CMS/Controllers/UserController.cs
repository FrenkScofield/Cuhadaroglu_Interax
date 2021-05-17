using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CMS.Controllers
{
    public class UserController : Controller
    {
        IUserService _IUserService;
        ISiteConfigService _ISiteConfigService;
        ISendMail _ISendMail;
        public UserController(IUserService _IUserService, ISendMail _ISendMail, ISiteConfigService _ISiteConfigService)
        {
            this._IUserService = _IUserService;
            this._ISendMail = _ISendMail;
            this._ISiteConfigService = _ISiteConfigService;
        }

        public ActionResult GetPaging([DataSourceRequest] DataSourceRequest request)
        {
            var orders = _IUserService.Where().Result;
            orders = orders.ApplyOrdersFiltering(request.Filters);
            var total = orders.Count();
            orders = orders.ApplyOrdersSorting(request.Groups, request.Sorts);
            if (!request.Sorts.Any() && !request.Groups.Any())
                orders = orders.OrderBy(o => o.Id);
            orders = orders.ApplyOrdersPaging(request.Page, request.PageSize);
            var data = orders.ApplyOrdersGrouping(request.Groups);
            var result = new DataSourceResult()
            {
                Data = data,
                Total = total
            };

            return Json(result);
        }

        public JsonResult GetUserType()
        {
            var list = Enum.GetValues(typeof(UserType)).Cast<int>().Select(x => new { name = ((UserType)x).ToStr(), value = x.ToString(), text = ((UserType)x).ExGetDescription() }).ToArray();
            return Json(list);
        }


        public IActionResult InsertOrUpdate(User postModel)
        {
            var result = _IUserService.InsertOrUpdate(postModel);
            return Json(result);
        }

        public User Get(int id)
        {
            var result = _IUserService.Find(id);
            return (result);
        }
        public IActionResult Approve(int id)
        {
            var resultUser = _IUserService.Find(id);


            if (resultUser != null)
            {
                resultUser.IsApproved = true;
                var rs = _IUserService.InsertOrUpdate(resultUser);
                _IUserService.SaveChanges();


                var config = _ISiteConfigService.Where(null, true, false, null).Result.FirstOrDefault();

                config.Mail = resultUser.UserName;

                config.SmtpMail = config.SmtpMail;
                config.SmtpMailPass = config.SmtpMailPass;
                config.SmtpHost = config.SmtpHost;
                config.SmtpPort = "587";
                config.MailGorunenAd = config.MailGorunenAd;
                config.SmtpSSL = false;


                var mailStr = "<!DOCTYPE html><html><head><style>th, td {  border: 1px solid #eee;}table{max-width:800px;}td{color:#000;}</style></head><body>";

                mailStr += "<table  width'100%'>";


                //http://interalcms.zonagency.com
                mailStr += "  <tr>";
                mailStr += "    <td>Sayın " + resultUser.Name + " " + resultUser.Surname + "</td>";
                mailStr += "  </tr>";
                mailStr += "  <tr>";
                mailStr += "    <td>Üyeliğiniz aktive edilmiştir. Mail adresiniz ile beraber belirlemiş olduğunuz şifre ile panelimize giriş yapabilir, ürünleri daha detaylı inceleyerek ihtiyacınız olan dosyaları indirebilirsiniz.</td>";
                mailStr += "  </tr>";
                mailStr += "</table></body></html>";

                string result = _ISendMail.Send(new MailModelCustom { Alicilar = new string[] { config.Mail }, cc = null, Icerik = mailStr, Konu = "Yeni Kullanıcı", MailGorunenAd = config.MailGorunenAd, SmtpHost = config.SmtpHost, SmtpMail = config.SmtpMail, SmtpMailPass = config.SmtpMailPass, SmtpPort = config.SmtpPort, SmtpSSL = config.SmtpSSL, SmtpUseDefaultCredentials = false });

                return Json("OK");
            }
            else
            {
                return Json("NO");

            }


            return Json("OK");
        }
        public IActionResult Delete(int id)
        {
            var result = _IUserService.Delete(id);
            _IUserService.SaveChanges();
            return Json(result);
        }

        public IActionResult Index()
        {
            ViewBag.pageTitle = "User";
            return View();
        }

        public IActionResult InsertOrUpdatePage()
        {
            ViewBag.postModel = Get(Request.Query["id"].ToInt());
            return View();
        }


    }
}

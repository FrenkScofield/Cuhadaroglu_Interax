using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;



namespace CMS.Controllers
{
    public class SiteConfigController : Controller
    {
        ISiteConfigService _ISiteConfigService;
        public SiteConfigController(ISiteConfigService _ISiteConfigService) { this._ISiteConfigService = _ISiteConfigService; }


        public JsonResult InsertOrUpdate(SiteConfig postModel)
        {
            var result = _ISiteConfigService.InsertOrUpdate(postModel);
            return Json(result);
        }

        public SiteConfig Get(int id)
        {
            var result = _ISiteConfigService.Where().Result.FirstOrDefault();
            return (result);
        }


        public IActionResult Index()
        {
            ViewBag.edit = Get(Request.Query["id"].ToInt());
            return View();
        }

        public IActionResult InsertOrUpdatePage()
        {
            return RedirectToAction("Index", "SiteConfig");
        }


    }
}

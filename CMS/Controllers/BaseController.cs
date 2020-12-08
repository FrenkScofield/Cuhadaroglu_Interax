using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;

namespace CMS.Controllers
{
    public class BaseController : Controller
    {
        ILangService _ILangService;
        IUserService _IUserService;
        IHostingEnvironment _IHostingEnvironment;
        IServiceConfigService _IServiceConfigService;
        IHttpContextAccessor _IHttpContextAccessor;

        public BaseController(
            IHostingEnvironment _IHostingEnvironment,
            IServiceConfigService _IServiceConfigService,
            IHttpContextAccessor _IHttpContextAccessor,
            ILangService _ILangService,
            IUserService _IUserService
            )
        {
            this._ILangService = _ILangService;
            this._IHostingEnvironment = _IHostingEnvironment;
            this._IServiceConfigService = _IServiceConfigService;
            this._IHttpContextAccessor = _IHttpContextAccessor;
            this._IUserService = _IUserService;

        }

        public IActionResult FileManager()
        {
            return View();
        }
        public IActionResult Index()
        {
            if (SessionRequest._User == null)
            {
              
                return RedirectToAction("Login1", "Login");
            }

            ViewBag.pageTitle = "Dashboard";



            var menus = _IServiceConfigService.Where().Result.ToList();

            var menuler = new List<string>();

            try
            {
                var filePath = _IHostingEnvironment.ContentRootPath + @"\Views";
                //menuler = Directory.EnumerateFiles(filePath, "*", SearchOption.AllDirectories).Select(o =>
                //o.Split("\\")[8].ToStr()

                //).Where(o =>
                //!o.ToStr().Contains("Base")
                //&& !o.ToStr().Contains("Shared")
                //&& !o.ToStr().Contains("Login")
                //&& !o.ToStr().Contains("_")
                //).Distinct().OrderBy(o => o).ToList();
            }
            catch (Exception ex)
            {
                //throw ex;
            }



            //var fark = menuler.Where(oo => !menus.Select(o => o.Name).Contains(oo)).ToList();

            //if (fark.Count > 0)
            //{

            //    fark.ForEach(o =>
            //    {
            //        menus.Add(new ServiceConfig()
            //        {
            //            Name = o,
            //            Description = o,
            //            Url = "/" + o,
            //            ServiceName = o
            //        });

            //    });
            //    _IServiceConfigService.AddBulk(menus);
            //    _IServiceConfigService.SaveChanges();

            //}

            _IHttpContextAccessor.HttpContext.Session.Set("menus", menus);

            return View();
        }

        public IActionResult Logout()
        {
            _IHttpContextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("Login1", "Login");
        }

        public IActionResult Report()
        {
            return View();
        }


    }
}

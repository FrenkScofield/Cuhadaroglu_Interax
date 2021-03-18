using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;



namespace CMS.Controllers
{
    public class ProjectProductController : Controller
    {
        IProjectProductService _IProjectProductService;
        IContentPageService _IContentPageService;
        public ProjectProductController(IProjectProductService _IProjectProductService, IContentPageService _IContentPageService)
        { this._IProjectProductService = _IProjectProductService; this._IContentPageService = _IContentPageService; }


        public IActionResult setData(int id1, int id2, string type) 
        {
            if (type == "add")
            {
                _IProjectProductService.Add(new ProjectProduct() { ProjectId = id1, ProductId = id2 });
            }
            else
            {
                var dp = _IProjectProductService.Where(o => o.ProjectId == id1 && o.ProductId == id2).Result.ToList();
                dp.ForEach(o =>
                {
                    _IProjectProductService.Delete(o);
                });
            }

            _IProjectProductService.SaveChanges();

            return Json("ok");
        }

        public IActionResult getData(int id1, int idFilter)
        {
            var mainList = _IProjectProductService.Where(o => o.ProjectId == id1).Result.ToList();
            var relationList = _IContentPageService.Where(o => o.ContentTypesId == idFilter).Result.ToList().Select(o =>
               new
               {
                   value = o.Id,
                   text = o.Name,
                   selected = mainList.Any(oo => oo.ProductId == o.Id)
               }).ToList();
            return Json(relationList);
        }


        public IActionResult Index()
        {
            ViewBag.pageTitle = "ProjectProduct";
            return View();
        }


    }
}

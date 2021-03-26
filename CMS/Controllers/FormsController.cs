using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using System.Linq;


namespace CMS.Controllers
{
    public class FormsController : Controller
    {
        IFormsService _IFormsService;
        public FormsController(IFormsService _IFormsService)
        {
            this._IFormsService = _IFormsService;
        }

        [HttpPost]
        public JsonResult GetPaging(DTParameters<Forms> param, int selectid)
        {
            var result = _IFormsService.GetPaging(o => o.FormTypeId == selectid, true, param, false, o => o.FormType);
            return Json(result);
        }



        public IActionResult InsertOrUpdate(Forms postModel)
        {
            var result = _IFormsService.InsertOrUpdate(postModel);
            return Json(result);
        }

        public Forms Get(int id)
        {
            var result = _IFormsService.Find(id);
            return (result);
        }

        public IActionResult Delete(int id)
        {
            var result = _IFormsService.Delete(id);
            _IFormsService.SaveChanges();
            return Json(result);
        }

        public IActionResult Index()
        {
            ViewBag.pageTitle = "Forms";
            return View();
        }

        public IActionResult InsertOrUpdatePage()
        {
            ViewBag.postModel = Get(Request.Query["id"].ToInt());
            return View();
        }


    }
}

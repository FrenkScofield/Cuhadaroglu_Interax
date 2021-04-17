using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CMS.Controllers
{
    public class SpecController : Controller
    {
        ISpecService _ISpecService;
        public SpecController(ISpecService _ISpecService)
        {
            this._ISpecService = _ISpecService;
        }

        [HttpPost]
        public JsonResult GetPaging(DTParameters<Spec> param, int selectid)
        {
            var result = _ISpecService.GetPaging(null, true, param, false, o => o.Parent, o => o.Orj, o => o.Lang);

            result.data = result.data.OrderBy(o => o.OrderNo).ToList();
            return Json(result);

        }

        [HttpPost]
        public JsonResult GetSelect(string name, string whereCase)
        {
            if (!string.IsNullOrEmpty(whereCase))
            {
                if (whereCase == "IsTanim")
                {
                    var result = _ISpecService.Where(o => o.IsTanim == true).Result
                   .Select(o => new { value = o.Id, text = o.Name + "(" + o.SpecType.ExGetDescription() + ")" }).ToList();
                    return Json(result);
                }
                else if (whereCase.ToInt() > 0)
                {
                    var result = _ISpecService.Where(o => (whereCase.ToInt() > 0 ? o.SpecType == (SpecType)whereCase.ToInt() : true)).Result
                  .Select(o => new { value = o.Id, text = o.Name + "(" + o.SpecType.ExGetDescription() + ")" }).ToList();
                    return Json(result);

                }
                else
                {
                    var result = _ISpecService.Where().Result
                    .Select(o => new { value = o.Id, text = o.Name + "(" + o.SpecType.ExGetDescription() + ")" }).ToList();
                    return Json(result);
                }

            }
            else
            {
                var result = _ISpecService.Where().Result
                  .Select(o => new { value = o.Id, text = o.Name + "(" + o.SpecType.ExGetDescription() + ")" }).ToList();
                return Json(result);
            }
        }

        public JsonResult GetSpecType()
        {
            var list = Enum.GetValues(typeof(SpecType)).Cast<int>().Select(x => new { name = ((SpecType)x).ToStr(), value = x.ToString(), text = ((SpecType)x).ExGetDescription() }).ToArray();
            return Json(list);
        }


        public IActionResult InsertOrUpdate(Spec postModel)
        {
            var result = _ISpecService.InsertOrUpdate(postModel);
            return Json(result);
        }

        public Spec Get(int id)
        {
            var result = _ISpecService.Find(id);
            return (result);
        }

        public IActionResult Delete(int id)
        {
            var result = _ISpecService.Delete(id);
            _ISpecService.SaveChanges();
            return Json(result);
        }

        public IActionResult Index()
        {
            ViewBag.pageTitle = "Spec";
            return View();
        }

        public IActionResult InsertOrUpdatePage()
        {
            ViewBag.postModel = Get(Request.Query["id"].ToInt());
            return View();
        }


        public IActionResult UpdateOrder(List<OrderUpdateModel> postModel)
        {
            var rows = _ISpecService.Where().Result.ToList();
            postModel.ForEach(o =>
            {
                var row = rows.FirstOrDefault(r => r.Id == o.Id);
                if (row != null)
                {
                    row.OrderNo = o.OrderNo;
                    _ISpecService.Update(row);
                    _ISpecService.SaveChanges();
                }
            });

            return Json("ok");
        }


    }
}

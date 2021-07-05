using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace CMSSite.Components
{

    public class ContentView : ViewComponent
    {

        IFormTypeService _IFormTypeService;
        IHttpContextAccessor _httpContextAccessor;
        IContentPageService _IContentPageService;
        ISpecService _ISpecService;
        IDocumentsService _IDocumentsService;
        IProjectProductService _IProjectProductService;
        public ContentView(
            IContentPageService _IContentPageService,
            ISpecService _ISpecService,
        IDocumentsService _IDocumentsService,
        IHttpContextAccessor httpContextAccessor,
        IFormTypeService _IFormTypeService,
        IProjectProductService _IProjectProductService
            )
        {
            this._IContentPageService = _IContentPageService;
            this._ISpecService = _ISpecService;
            this._httpContextAccessor = httpContextAccessor;
            this._IDocumentsService = _IDocumentsService;
            this._IFormTypeService = _IFormTypeService;
            this._IProjectProductService = _IProjectProductService;
        }

        public IViewComponentResult Invoke(TemplateType TemplateType)
        {
            var link = HttpContext.Request.Path.Value.Trim('/').ToStr();

            if (_httpContextAccessor.HttpContext.Session.Get("fUser") != null)
            {
                User currUser = _httpContextAccessor.HttpContext.Session.Get<User>("fUser");
                ViewBag.User = currUser;
            }
            else
            {
                ViewBag.User = null;
            }

            List<ContentPage> contentPages = new List<ContentPage>();
            var content = _IContentPageService.Where(o => o.Link.ToLower() == link.ToLower() && o.IsActive == true, true, false,
           o => o.Parent,o => o.Picture,
                o => o.BannerImage,
                o => o.SpecContentValue,o=>o.FormType)
                .Result.FirstOrDefault();



            //if (content.FormTypeId)
            //{
            //    int fid = content.FormTypeId.Value;
            //    FormType formtip  _IFormTypeService.Where(o => o.Id == fid).Result.FirstOrDefault();
            //    ViewBag.formtip = formtip;

            //}
            if (content.ContentTypesId == 5)
            {
                ViewBag.ProductList = _IProjectProductService.Where(o => o.ProjectId == content.Id, true, false, o => o.Product).Result.ToList();
            }
            if (content.ContentTypesId == 3)
            {
                ViewBag.ProjectList = _IProjectProductService.Where(o => o.ProductId == content.Id, true, false, o => o.Project, o => o.Project.ThumbImage).Result.ToList();
            }

            var Specs = _ISpecService.Where(null, true, false, o => o.Parent, o => o.SpecAttrs).Result.ToList();
            ViewBag.Specs = Specs;
            int langID = content != null ? content.LangId : 1;
            if (content != null && _httpContextAccessor.HttpContext.Session.GetInt32("LanguageID") != content.LangId)
            {
                _httpContextAccessor.HttpContext.Session.SetInt32("LanguageID", content.LangId);
                langID = content.LangId;
            }

            ViewBag.content = content;

            Regex reg = new Regex("[*'\",_&#^@]");
            var currState = reg.Replace(_httpContextAccessor.HttpContext.Session.GetString("currState") ?? "-", string.Empty);

            ViewBag.currState = currState;
            bool isBayi = false;
            bool isEndustri = false;
            bool isMimar = false;
            bool isBireysel = false;

            contentPages = _IContentPageService.Where(x => x.LangId == langID && x.IsDeleted == null && x.IsActive == true && x.IsInteral == true, true, false,
                o => o.ContentPageChilds, o => o.SpecContentValue, o => o.Parent,  o => o.ThumbImage).Result.ToList();
            _httpContextAccessor.HttpContext.Session.Set("contentPages", contentPages.Where(o => o.LangId == langID));
            ViewBag.contentPages = contentPages;

            List<ContentPage> FilteredCP = new List<ContentPage>();
            ViewBag.LanguageID = langID;
            ViewBag.Pages = contentPages.ToList();
            switch (currState)
            {
                case "Uygulayıcı":
                    isBayi = true;
                    FilteredCP = contentPages.Where(x => x.IsBayi == isBayi).ToList();
                    ViewBag.contentPages = FilteredCP;
                    break;
                case "Endüstriyel":
                    isEndustri = true;
                    FilteredCP = contentPages.Where(x => x.IsEndustri == isEndustri).ToList();
                    ViewBag.contentPages = FilteredCP;
                    break;
                case "Mimarlar":
                    isMimar = true;
                    FilteredCP = contentPages.Where(x => x.IsMimar == isMimar).ToList();
                    ViewBag.contentPages = FilteredCP;
                    break;
                case "Bireysel":
                    isBireysel = true;
                    FilteredCP = contentPages.Where(x => x.IsBireysel == isBireysel).ToList();
                    ViewBag.contentPages = FilteredCP;
                    break;
                case "-":
                    isMimar = true;
                    FilteredCP = contentPages.Where(x => x.IsMimar == isMimar).ToList();
                    ViewBag.contentPages = FilteredCP;
                    break;
                default:
                    isMimar = true;
                    break;
            }




            if (TemplateType == TemplateType.ProjeListeleme)
            {

                List<ContentPage> currList = contentPages.Where(x => x.ContentPageId == content.Id && x.IsDeleted == null).ToList();
                List<int> currSpecList = currList.SelectMany(x => x.SpecContentValue).Select(s => s.SpecId).Distinct().ToList();
                //ViewBag.contentPages = contentPages.Where(x => currContentList.Contains(x.ContentPageId ?? 0) && x.IsDeleted == null).OrderBy(o => o.ContentOrderNo).ToList();

                ViewBag.subPages = currList;
                ViewBag.currSpecList = currSpecList;


            }
            else if (TemplateType == TemplateType.UrunDetay)
            {

                if (ViewBag.contentDetails == null)
                {
                    var contentDetails = _IContentPageService.Where(o => o.Link.ToLower() == link.ToLower() && o.IsActive == true, true, false,
                   o => o.TechnicalProperties,
                   o => o.TechnicalDocuments,
                   o => o.CadDatas,
                   o => o.BIMFiles,
                o => o.Gallery,
                o => o.Documents
                   )
                   .Result.FirstOrDefault();

                    ViewBag.contentDetails = contentDetails;
                }
            }
            else if (TemplateType == TemplateType.BlogListeleme)
            {
                ViewBag.subPages = contentPages.Where(x => x.ContentPageId == 16).ToList();
            }
            else if (TemplateType == TemplateType.UrunListeleme)
            {

                List<ContentPage> currList = FilteredCP.Where(x => x.ContentPageId == content.Id && x.IsDeleted == null).ToList();
                List<int> currListIds = currList.Select(x => x.Id).ToList();
                List<int> relSpecListIds = Specs.Where(x => x.ParentId != 15).Select(s => s.Id).ToList();
                //Spec List
                var currSpecListObj = currList.SelectMany(x => x.SpecContentValue).Where(k => relSpecListIds.Contains(k.SpecId) && currListIds.Contains(k.ContentPageId) && k.ContentValue == "true");
                List<int> currSpecList = currSpecListObj.Select(s => s.SpecId).Distinct().ToList();

                ViewBag.categories = contentPages.Where(x => x.ContentPageId == content.Parent.Id && x.IsDeleted == null).ToList();
                //ViewBag.contentPages = contentPages.Where(x => currContentList.Contains(x.ContentPageId ?? 0) && x.IsDeleted == null).OrderBy(o => o.ContentOrderNo).ToList();
                ViewBag.subPages = currList;
                ViewBag.currSpecList = currSpecList;
                //select Distinct SpecId from SpecContentValue


            }
            else if (TemplateType == TemplateType.KategoriListeleme)
            {

                List<ContentPage> currList = contentPages.Where(x => x.ContentPageId == content.Id && x.IsDeleted == null).ToList();
                List<int> currSpecList = currList.SelectMany(x => x.SpecContentValue).Select(s => s.SpecId).Distinct().ToList();
                ViewBag.categories = contentPages.Where(x => x.ContentPageId == content.Parent?.Id && x.IsDeleted == null).ToList();
                //ViewBag.contentPages = contentPages.Where(x => currContentList.Contains(x.ContentPageId ?? 0) && x.IsDeleted == null).OrderBy(o => o.ContentOrderNo).ToList();
                ViewBag.subPages = currList;
                ViewBag.currSpecList = currSpecList;

                //select Distinct SpecId from SpecContentValue
            }
            return View(TemplateType.ToString());
        }
    }
}